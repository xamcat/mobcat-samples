using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using PushDemo.Droid.Services;
using Android.Content;
using PushDemo.Services;
using System;
using Firebase.Iid;
using Android.Gms.Common;

namespace PushDemo.Droid
{
    [Activity(Label = "PushDemo", LaunchMode = LaunchMode.SingleTop, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        IPushDemoNotificationActionService _notificationActionService;

        IPushDemoNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushDemoNotificationActionService>());

        bool PlayServicesAvailable
            => GoogleApiAvailability.Instance
                .IsGooglePlayServicesAvailable(this) == ConnectionResult.Success;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            if (PlayServicesAvailable)
            {
                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                    .GetInstanceId()
                    .AddOnSuccessListener(this);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Bootstrap.Begin(() => new DeviceInstallationService(
                () => PushNotificationFirebaseMessagingService.Token,
                () => PlayServicesAvailable,
                () => GetPlayServicesError()));

            LoadApplication(new App());

            ProcessNotificationActions(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessNotificationActions(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnSuccess(Java.Lang.Object result)
            => PushNotificationFirebaseMessagingService.Token =
                result.Class.GetMethod("getToken").Invoke(result).ToString();

        void ProcessNotificationActions(Intent intent)
        {
            try
            {
                if (intent?.HasExtra("action") == true)
                {
                    var action = intent.GetStringExtra("action");

                    if (!string.IsNullOrEmpty(action))
                        NotificationActionService.TriggerAction(action);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        string GetPlayServicesError()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);

            if (resultCode != ConnectionResult.Success)
                return GoogleApiAvailability.Instance.IsUserResolvableError(resultCode) ?
                           GoogleApiAvailability.Instance.GetErrorString(resultCode) :
                           "This device is not supported";

            return "An error occurred preventing the use of push notifications";
        }
    }
}