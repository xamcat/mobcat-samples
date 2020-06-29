using System;
using Android.App;
using Android.Gms.Common;
using PushDemo.Models;
using PushDemo.Services;
using static Android.Provider.Settings;

namespace PushDemo.Droid.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        public string Token { get; set; }

        public bool NotificationsSupported
            => GoogleApiAvailability.Instance
                .IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

        public string GetDeviceId()
            => Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);

        public DeviceInstallation GetDeviceInstallation(params string[] tags)
        {
            if (!NotificationsSupported)
                throw new Exception(GetPlayServicesError());

            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Unable to resolve token for FCM");

            var installation = new DeviceInstallation
            {
                InstallationId = GetDeviceId(),
                Platform = "fcm",
                PushChannel = Token
            };

            installation.Tags.AddRange(tags);

            return installation;
        }

        string GetPlayServicesError()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);

            if (resultCode != ConnectionResult.Success)
                return GoogleApiAvailability.Instance.IsUserResolvableError(resultCode) ?
                           GoogleApiAvailability.Instance.GetErrorString(resultCode) :
                           "This device is not supported";

            return "An error occurred preventing the use of push notifications";
        }
    }
}