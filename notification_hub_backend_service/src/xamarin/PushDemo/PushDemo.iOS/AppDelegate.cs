using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using PushDemo.iOS.Extensions;
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
        IPushDemoNotificationActionService _notificationActionService;
        INotificationRegistrationService _notificationRegistrationService;
        IDeviceInstallationService _deviceInstallationService;

        IPushDemoNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushDemoNotificationActionService>());

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
                (_notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
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

        Task CompleteRegistrationAsync(NSData deviceToken)
        {
            DeviceInstallationService.Token = deviceToken.ToHexString();

            return NotificationRegistrationService.RefreshRegistrationAsync();
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
    }
}