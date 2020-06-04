using System;
using System.Threading.Tasks;
using BiometricAuthentication.NetCore;
using Foundation;
using LocalAuthentication;
using UIKit;

namespace BiometricAuthentication.iOS
{
    public class BiometricAuthenticationService : IBiometricAuthenticationService
    {
        private bool _hasEvaluatedBiometricType;
        private BiometricType _biometricType;
        string BiometryType = "";
        string localizedReason = new NSString("To access secrets");

        /// <inheritdoc />
        public Task<AuthenticationResult> AuthenticateAsync(string alertMessage = null)
        {
            if (AvailableBiometricType == BiometricType.None)
            {
                Console.WriteLine("[BiometricAthenticationService] Authentication not available on this device");
                return Task.FromResult(new AuthenticationResult(false, "Authentication not available"));
            }

            var tcs = new TaskCompletionSource<AuthenticationResult>();

            var context = new LAContext();
            NSError authError;

            // Because LocalAuthentication APIs have been extended over time,
            // you must check iOS version before setting some properties
            context.LocalizedFallbackTitle = "Fallback";

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                context.LocalizedCancelTitle = "Cancel";
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                context.LocalizedReason = "Authorize for access to secrets";
                BiometryType = context.BiometryType == LABiometryType.TouchId ? "TouchID" : "FaceID";
                Console.WriteLine(BiometryType);
            }

            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out authError))
            {
                var replyHandler = new LAContextReplyHandler((success, error) =>
                {
                    //Make sure it runs on MainThread, not in Background
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            System.Diagnostics.Debug.WriteLine("Authentication Success");
                            tcs.TrySetResult(new AuthenticationResult(true));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Authentication Failure : " + error.Description);
                            tcs.TrySetResult(new AuthenticationResult(false, error.Description));
                        }
                    });
                });

                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, alertMessage ?? localizedReason, replyHandler);
            }

            else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out authError))
            {
                var replyHandler = new LAContextReplyHandler((success, error) =>
                {
                    //Make sure it runs on MainThread, not in Background
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            System.Diagnostics.Debug.WriteLine("Authentication Success");
                            tcs.TrySetResult(new AuthenticationResult(true));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Authentication Failure : " + error.Description);
                            tcs.TrySetResult(new AuthenticationResult(false, error.Description));
                        }
                    });
                });

                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, alertMessage ?? localizedReason, replyHandler);
            }
            else
            {
                //No Auth setup on Device
                Console.WriteLine($"This device doesn't have authentication configured: {authError.ToString()}");
                tcs.TrySetResult(new AuthenticationResult(false, "This device does't have authentication configured."));
            }

            return tcs.Task;
        }

        /// <inheritdoc />
        public BiometricType AvailableBiometricType
        {
            get
            {
                if (_hasEvaluatedBiometricType) return _biometricType;

                var localAuthContext = new LAContext();
                NSError authError;

                if (localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out authError))
                {
                    if (localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out authError))
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                        {
                            _biometricType = localAuthContext.BiometryType == LABiometryType.TouchId ? BiometricType.Fingerprint : BiometricType.Face;
                            Console.WriteLine(_biometricType);
                            _hasEvaluatedBiometricType = true;
                            return _biometricType;
                        }
                    }

                    Console.WriteLine($"[BiometricAuthenticationService] Local Auth context failed with error: {authError}");

                    _biometricType = BiometricType.Passcode;
                    _hasEvaluatedBiometricType = true;
                    return _biometricType;
                }

                Console.WriteLine($"[BiometricAuthenticationService] Local Auth context failed with error: {authError}");

                _biometricType = BiometricType.None;
                _hasEvaluatedBiometricType = true;
                return _biometricType;
            }
        }

        private int GetOsMajorVersion()
        {
            return int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
        }
    }
}
