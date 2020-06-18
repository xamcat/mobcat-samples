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
        Func<bool> _notificationsSupported;
        Func<string> _getNotificationSupportError;

        public DeviceInstallationService(
            Func<NSData> getDeviceToken,
            Func<bool> notificationsSupported,
            Func<string> getNotificationSupportError)
        {
            _getDeviceToken = getDeviceToken ?? throw new ArgumentException(
                $"Parameter {nameof(getDeviceToken)} cannot be null");

            _notificationsSupported = notificationsSupported ?? throw new ArgumentException(
                $"Parameter {nameof(notificationsSupported)} cannot be null");

            _getNotificationSupportError = getNotificationSupportError ?? throw new ArgumentException(
                $"Parameter {nameof(getNotificationSupportError)} cannot be null");
        }

        public string GetDeviceId()
            => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            if (!_notificationsSupported())
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