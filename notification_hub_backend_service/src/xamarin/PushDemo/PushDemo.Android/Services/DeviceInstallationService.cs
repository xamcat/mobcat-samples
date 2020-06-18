using System;
using Android.App;
using PushDemo.Models;
using PushDemo.Services;
using static Android.Provider.Settings;

namespace PushDemo.Droid.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        Func<string> _getFirebaseToken;
        Func<bool> _playServicesAvailable;
        Func<string> _getPlayServicesError;

        public string GetDeviceId()
            => Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);

        public DeviceInstallationService(
            Func<string> getFirebaseToken,
            Func<bool> playServicesAvailable,
            Func<string> getPlayServicesError)
        {
            _getFirebaseToken = getFirebaseToken ?? throw new ArgumentException(
                $"Parameter {nameof(getFirebaseToken)} cannot be null");

            _playServicesAvailable = playServicesAvailable ?? throw new ArgumentException(
                $"Parameter {nameof(playServicesAvailable)} cannot be null");

            _getPlayServicesError = getPlayServicesError ?? throw new ArgumentException(
                $"Parameter {nameof(getPlayServicesError)} cannot be null");
        }

        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            if (!_playServicesAvailable())
                throw new Exception(_getPlayServicesError());

            var installationId = GetDeviceId();
            var token = _getFirebaseToken();

            if (token == null)
                return null;

            var installation = new DeviceInstallation
            {
                InstallationId = installationId,
                Platform = "fcm",
                PushChannel = token
            };

            installation.Tags.AddRange(tags);

            return installation;
        }
    }
}