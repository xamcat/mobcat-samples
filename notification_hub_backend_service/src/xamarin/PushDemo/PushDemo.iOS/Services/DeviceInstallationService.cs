using System;
using Foundation;
using PushDemo.Models;
using PushDemo.Services;
using UIKit;

namespace PushDemo.iOS.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        Func<NSData> _getDeviceToken;
        Func<bool> _notifificationsSupported;
        Func<string> _getNotificationSupportError;

        public DeviceInstallationService(
            Func<NSData> getDeviceToken,
            Func<bool> notificationsSupported,
            Func<string> getNotificationSupportError)
        {
            _getDeviceToken = getDeviceToken ?? throw new ArgumentException(
                    $"Parameter {nameof(getDeviceToken)} cannot be null");

            _notifificationsSupported = notificationsSupported ?? throw new ArgumentException(
                    $"Parameter {nameof(notificationsSupported)} cannot be null");

            _getNotificationSupportError = getNotificationSupportError ?? throw new ArgumentException(
                    $"Parameter {nameof(getNotificationSupportError)} cannot be null");
        }

        public string GetDeviceId()
            => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            if (!_notifificationsSupported())
                throw new Exception(_getNotificationSupportError());

            var installationId = GetDeviceId();
            var token = _getDeviceToken();

            if (token == null)
                return null;

            var pushChannel = NSDataToHex(token);

            var installation = new DeviceInstallation
            {
                InstallationId = installationId,
                Platform = "apns",
                PushChannel = pushChannel
            };

            installation.Tags.AddRange(tags);

            PushTemplate genericTemplate = new PushTemplate
            {
                Body = "{\"aps\":{\"alert\":\"$(alertMessage)\"}, \"action\": \"$(alertAction)\"}"
            };

            PushTemplate silentTemplate = new PushTemplate
            {
                Body = "{\"aps\":{\"content-available\":1, \"apns-priority\": 5, \"sound\":\"\", \"badge\": 0}, \"message\": \"$(silentMessage)\", \"action\": \"$(silentAction)\"}"
            };

            installation.Templates.Add("genericTemplate", genericTemplate);
            installation.Templates.Add("silentTemplate", silentTemplate);

            return installation;
        }

        string NSDataToHex(NSData data) => ByteToHex(data.ToArray());

        string ByteToHex(byte[] data)
        {
            if (data == null)
                return null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Length * 2);

            foreach (byte b in data)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString().ToUpperInvariant();
        }
    }
}