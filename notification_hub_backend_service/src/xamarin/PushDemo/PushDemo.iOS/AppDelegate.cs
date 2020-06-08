using System;
using System.Diagnostics;
using Foundation;
using PushDemo.iOS.Services;
using PushDemo.Services;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;

namespace PushDemo.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        const string CachedDeviceToken = "CachedDeviceToken";
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
                RequestRemoteNotifications();

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
        {
            _deviceToken = deviceToken;

            SecureStorage.GetAsync(CachedDeviceToken).ContinueWith((i) =>
            {
                if (i.IsFaulted) throw i.Exception;

                var cachedToken = i.Result;

                var deviceTokenHash = _deviceToken?.Description?.GetHashCode().ToString();

                if (!string.IsNullOrWhiteSpace(cachedToken) &&
                    cachedToken.Equals(deviceTokenHash))
                    return;

                NotificationRegistrationService.RefreshRegistrationAsync().ContinueWith((j) =>
                {
                    if (j.IsFaulted) throw j.Exception;

                    SecureStorage.SetAsync(CachedDeviceToken, deviceTokenHash).ContinueWith((k)
                        => { if (k.IsFaulted) throw k.Exception; });
                });
            });
        }

        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
            => Debug.WriteLine(error.Description); 

        public override void ReceivedRemoteNotification(
            UIApplication application,
            NSDictionary userInfo)
            => ProcessNotificationActions(userInfo);

        void RequestRemoteNotifications()
        {
            UNUserNotificationCenter.Current.GetNotificationSettings((settings) => {
                var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                var badgeAllowed = (settings.BadgeSetting == UNNotificationSetting.Enabled);
                var soundAllowed = (settings.SoundSetting == UNNotificationSetting.Enabled);

                if (!alertsAllowed || !badgeAllowed || !soundAllowed)
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
                else
                {
                    RegisterForRemoteNotifications();
                }
            });
        }

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