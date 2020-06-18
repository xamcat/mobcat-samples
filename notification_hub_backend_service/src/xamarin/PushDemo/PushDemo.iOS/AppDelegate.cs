using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using PushDemo.iOS.Services;
using PushDemo.Services;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;

namespace PushDemo.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        const string CachedDeviceToken = "cached_device_token";
        const int SupportedVersionMajor = 13;
        const int SupportedVersionMinor = 0;

        NSData _deviceToken;
        IPushDemoNotificationActionService _notificationActionService;
        INotificationRegistrationService _notificationRegistrationService;

        IPushDemoNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushDemoNotificationActionService>());

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
                (_notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>());

        bool NotificationsSupported
            => UIDevice.CurrentDevice.CheckSystemVersion(SupportedVersionMajor, SupportedVersionMinor);

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            if (NotificationsSupported)
            {
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert |
                        UNAuthorizationOptions.Badge |
                        UNAuthorizationOptions.Sound,
                        (approvalGranted, error) =>
                        {
                            if (approvalGranted && error == null)
                                RegisterForRemoteNotifications();
                        });
            }

            Bootstrap.Begin(() => new DeviceInstallationService(
                () => _deviceToken,
                () => NotificationsSupported,
                () => GetNotificationsSupportError()));

            LoadApplication(new App());

            using (var userInfo = options?.ObjectForKey(
                UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary)
                    ProcessNotificationActions(userInfo);

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(
            UIApplication application,
            NSData deviceToken)
            => CompleteRegistrationAsync(deviceToken).ContinueWith((task)
                => { if (task.IsFaulted) throw task.Exception; });

        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
            => Debug.WriteLine(error.Description); 

        public override void ReceivedRemoteNotification(
            UIApplication application,
            NSDictionary userInfo)
            => ProcessNotificationActions(userInfo);

        void RegisterForRemoteNotifications()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert |
                    UIUserNotificationType.Badge |
                    UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            });
        }

        async Task CompleteRegistrationAsync(NSData deviceToken)
        {
            _deviceToken = deviceToken;

            var cachedToken = await SecureStorage.GetAsync(CachedDeviceToken)
                .ConfigureAwait(false);

            var tokenHash = _deviceToken?.Description?.GetHashCode().ToString();

            if (!string.IsNullOrWhiteSpace(cachedToken) &&
                cachedToken.Equals(tokenHash))
                return;

            await NotificationRegistrationService.RefreshRegistrationAsync()
                .ConfigureAwait(false);

            await SecureStorage.SetAsync(CachedDeviceToken, tokenHash)
                .ConfigureAwait(false);
        }

        void ProcessNotificationActions(NSDictionary userInfo)
        {
            if (userInfo == null)
                return;

            try
            {
                var actionValue = userInfo.ObjectForKey(new NSString("action")) as NSString;

                if (!string.IsNullOrWhiteSpace(actionValue?.Description))
                    NotificationActionService.TriggerAction(actionValue.Description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        string GetNotificationsSupportError()
        {
            if (!NotificationsSupported)
                return $"This app only supports notifications on iOS {SupportedVersionMajor}.{SupportedVersionMinor} and above. You are running {UIDevice.CurrentDevice.SystemVersion}.";

            if (_deviceToken == null)
                return $"This app can support notifications but you must enable this in your settings.";


            return "An error occurred preventing the use of push notifications";
        }
            
    }
}