using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Firebase.Messaging;
using PushDemo.Services;

namespace PushDemo.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT", "FLAG_INCLUDE_STOPPED_PACKAGES" })]
    public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
    {
        const string ChannelId = nameof(PushNotificationFirebaseMessagingService);

        Random _random;

        NotificationManager _notificationManager;
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

        internal static bool AppInForeground { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();

            _random = new Random();
            _notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel(
                    ChannelId,
                    "PushDemo",
                    NotificationImportance.Default);

                _notificationManager.CreateNotificationChannel(channel);
            }
        }

        public override void OnNewToken(string token)
        {
            Token = token;

            NotificationRegistrationService.RefreshRegistrationAsync()
                .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; });
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            message.Data.TryGetValue("action", out var messageAction);

            if (!AppInForeground &&
                message.Data.TryGetValue("message", out var messageBody))
            {
                if (string.IsNullOrWhiteSpace(messageBody) ||
                    (message.Data.TryGetValue("silent", out var silentString) &&
                    bool.TryParse(silentString, out var silent) && silent))
                    return; 

                SendNotification("PushDemo", messageBody, messageAction); 
            }

            if (AppInForeground && !string.IsNullOrWhiteSpace(messageAction))
                NotificationActionService.TriggerAction(messageAction);
        }

        void SendNotification(string title, string body, string action)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra(nameof(body), body);
            intent.PutExtra(nameof(action), action);

            var pendingIntent = PendingIntent.GetActivity(
                this,
                _random.Next(),
                intent,
                PendingIntentFlags.UpdateCurrent);

            var builder = new NotificationCompat.Builder(this, ChannelId)
                          .SetAutoCancel(true) 
                          .SetContentIntent(pendingIntent) 
                          .SetContentTitle(title) 
                          .SetSmallIcon(Resource.Mipmap.icon) 
                          .SetContentText(body); 

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(_random.Next(), builder.Build());
        }
    }
}