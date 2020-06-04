using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Views;
using Android.Widget;
using BiometricAuthentication.NetCore;
using Java.Lang;
using Javax.Crypto;
using CancellationSignal = Android.Support.V4.OS.CancellationSignal;

namespace BiometricAuthentication.Droid
{
    public class FingerprintManagerFragment : DialogFragment, IDialogInterfaceOnShowListener
    {
        private readonly FingerprintManagerCompat _fingerprintManager;
        private readonly WeakReference<TaskCompletionSource<AuthenticationResult>> _tcsWeak;
        private readonly string _alertTitle;
        private readonly string _alertMessage;
        private readonly CryptoObjectHelper _cryptObjectHelper;
        private CancellationSignal _cancellationSignal;

        internal FingerprintManagerFragment(string alertTitle, string alertMessage, TaskCompletionSource<AuthenticationResult> tcs)
        {
            _fingerprintManager = FingerprintManagerCompat.From(MainApplication.CurrentActivity);
            _alertTitle = alertTitle;
            _alertMessage = alertMessage;
            _cryptObjectHelper = new CryptoObjectHelper();
            _cancellationSignal = new CancellationSignal();
            _tcsWeak = new WeakReference<TaskCompletionSource<AuthenticationResult>>(tcs);
        }

        public FingerprintManagerFragment(TaskCompletionSource<AuthenticationResult> tcs)
            : this(null, null, tcs)
        {
        }

        public void OnShow(IDialogInterface dialog)
        {
            _fingerprintManager.Authenticate(
                _cryptObjectHelper.BuildCryptoObject(),
                (int)FingerprintAuthenticationFlags.None,
                _cancellationSignal,
                new SimpleAuthCallbacks(dialog, _tcsWeak, _cancellationSignal),
                null
            );
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var context = MainApplication.CurrentActivity;

            var alertBuilder = CreateDialog(context);

            var alert = alertBuilder.Create();

            alert.SetOnShowListener(this);

            return alert;
        }

        /// <summary>
        /// Override this to create your own custom authentication dialog
        /// </summary>
        /// <returns>The dialog.</returns>
        /// <param name="context">Context.</param>
        public virtual AlertDialog.Builder CreateDialog(Context context)
        {
            var builder = new AlertDialog.Builder(context);

            builder.SetTitle(_alertTitle);
            builder.SetMessage(_alertMessage);
            builder.SetNegativeButton("Cancel", CancelledDialogDelegate);

            var linearLayout = new LinearLayout(context);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.SetHorizontalGravity(GravityFlags.Center);

            var imageView = new ImageView(context);
            imageView.SetImageResource(Resource.Drawable.ic_fp_40px);
            imageView.SetForegroundGravity(GravityFlags.Center);
            imageView.LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );

            linearLayout.AddView(imageView);
            builder.SetView(linearLayout);

            return builder;
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);

            CancelledDialogDelegate(null, null);
        }

        protected void CancelledDialogDelegate(object sender, DialogClickEventArgs e)
        {
            if (_tcsWeak.TryGetTarget(out var tcs))
                tcs.TrySetResult(new AuthenticationResult(false, "Biometric authentication scan was cancelled"));
            _cancellationSignal?.Cancel();
            _cancellationSignal = null;
        }

        private class SimpleAuthCallbacks : FingerprintManagerCompat.AuthenticationCallback
        {
            static readonly byte[] SecretBytes = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            readonly IDialogInterface _dialog;
            readonly WeakReference<TaskCompletionSource<AuthenticationResult>> _tcsWeak;
            CancellationSignal _cancellationSignal;

            public SimpleAuthCallbacks(
                IDialogInterface dialog,
                WeakReference<TaskCompletionSource<AuthenticationResult>> tcsWeak,
                CancellationSignal cancellationSignal)
            {
                _dialog = dialog;
                _tcsWeak = tcsWeak;
                _cancellationSignal = cancellationSignal ?? new CancellationSignal();
            }

            public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
            {
                Console.WriteLine("OnAuthenticationSucceeded");
                string errorMessage = "Encryption Error : ";

                if (result.CryptoObject.Cipher != null)
                {
                    try
                    {
                        byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SecretBytes);
                        ReportSuccess();
                    }

                    catch (BadPaddingException bpe)
                    {
                        errorMessage += "Failed to encrypt the data with the generated key." + bpe;
                        Console.WriteLine(errorMessage);
                        ReportAuthenticationFailed(errorMessage);
                    }
                    catch (IllegalBlockSizeException ibse)
                    {
                        errorMessage += "Failed to encrypt the data with the generated key." + ibse;
                        Console.WriteLine(errorMessage);
                        ReportAuthenticationFailed(errorMessage);
                    }
                }
                else
                {
                    Console.WriteLine("Fingerprint authentication succeeded.");
                    ReportSuccess();
                }
            }

            void ReportSuccess()
            {
                System.Diagnostics.Debug.WriteLine("Auth Success");
                if (_tcsWeak.TryGetTarget(out var tcs))
                    tcs.TrySetResult(new AuthenticationResult(true));
                _dialog.Dismiss();
            }

            void ReportScanFailure(string errorMessage)
            {
                System.Diagnostics.Debug.WriteLine(errorMessage);
                if (_tcsWeak.TryGetTarget(out var tcs))
                    tcs.TrySetResult(new AuthenticationResult(false, errorMessage));
                _dialog.Dismiss();
            }

            void ReportAuthenticationFailed(string errorMessage)
            {
                System.Diagnostics.Debug.WriteLine(errorMessage);
                if (_tcsWeak.TryGetTarget(out var tcs))
                    tcs.TrySetResult(new AuthenticationResult(false, errorMessage));
                _cancellationSignal.Cancel();
                _dialog.Dismiss();
            }

            public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
            {
                bool reportError = (errMsgId == (int)FingerprintState.ErrorCanceled);
                string debugMsg = $"OnAuthenticationError: {errMsgId}:`{errString}`.";

                if (reportError)
                {
                    debugMsg += (errMsgId, errString.ToString());
                    debugMsg += " Reporting the error.";
                }
                else
                {
                    debugMsg += " Ignoring the error.";
                }

                Console.WriteLine(debugMsg);
                if (_tcsWeak.TryGetTarget(out var tcs))
                    tcs.TrySetResult(new AuthenticationResult(false, debugMsg));

                _cancellationSignal?.Cancel();
                _cancellationSignal?.Dispose();
                _cancellationSignal = null;

                _dialog.Dismiss();
            }


            public override void OnAuthenticationFailed()
            {
                Console.WriteLine("Authentication failed.");
                ReportAuthenticationFailed("Authentication Failed, Fingerprint not recognized");
            }

            public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
            {
                string errorMessage = $"OnAuthenticationHelp: {helpString}:`{helpMsgId}`";
                Console.WriteLine(errorMessage);
                ReportScanFailure(errorMessage);
            }
        }
    }
}
