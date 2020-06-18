using Android.App;
using Android.Content;
using Firebase.Messaging;
using PushDemo.Services;

namespace PushDemo.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
    {
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

        internal static string Token { get; set; }

        public override void OnNewToken(string token)
        {
            Token = token;

            NotificationRegistrationService.RefreshRegistrationAsync()
                .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; });
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            if(message.Data.TryGetValue("action", out var messageAction))
                NotificationActionService.TriggerAction(messageAction);
        }
    }
}