using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushDemoApi.Models;

namespace PushDemoApi.Services
{
    public class NotificationHubService : INotificationService
    {
        readonly NotificationHubClient _hub;
        readonly Dictionary<string, NotificationPlatform> _installationPlatform;
        readonly ILogger<NotificationHubService> _logger;

        public NotificationHubService(IOptions<NotificationHubOptions> options, ILogger<NotificationHubService> logger)
        {
            _logger = logger;
            _hub = NotificationHubClient.CreateClientFromConnectionString(
                options.Value.ConnectionString,
                options.Value.Name);

            _installationPlatform = new Dictionary<string, NotificationPlatform>
            {
                { nameof(NotificationPlatform.Apns).ToLower(), NotificationPlatform.Apns },
                { nameof(NotificationPlatform.Fcm).ToLower(), NotificationPlatform.Fcm }
            };
        }

        public async Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.Platform) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.PushChannel))
                return false;

            var templates = deviceInstallation.Templates
                .ToDictionary(
                    i => i.Key,
                    i => new InstallationTemplate
                    {
                        Body = i.Value.Body
                    });

            var installation = new Installation()
            {
                InstallationId = deviceInstallation.InstallationId,
                PushChannel = deviceInstallation.PushChannel,
                Tags = deviceInstallation.Tags,
                Templates = templates
            };

            if (_installationPlatform.TryGetValue(deviceInstallation.Platform, out var platform))
                installation.Platform = platform;
            else
                return false;

            try
            {
                await _hub.CreateOrUpdateInstallationAsync(installation, token);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(installationId))
                return false;

            try
            {
                await _hub.DeleteInstallationAsync(installationId, token);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token)
        {
            if ((notificationRequest.Silent &&
                string.IsNullOrWhiteSpace(notificationRequest?.Action)) ||
                (!notificationRequest.Silent &&
                (string.IsNullOrWhiteSpace(notificationRequest?.Text)) ||
                string.IsNullOrWhiteSpace(notificationRequest?.Action)))
                return false;

            var templateParams = notificationRequest.Silent ?
                new Dictionary<string, string>
                {
                    { "silentMessage", notificationRequest.Text },
                    { "silentAction", notificationRequest.Action }
                } :
                new Dictionary<string, string>
                {
                    { "alertMessage", notificationRequest.Text },
                    { "alertAction", notificationRequest.Action }
                };

            try
            {
                if (notificationRequest.Tags.Length == 0)
                {
                    // This will broadcast to all users registered in the notification hub
                    await _hub.SendTemplateNotificationAsync(templateParams, token);
                }
                else if (notificationRequest.Tags.Length <= 20)
                {
                    await _hub.SendTemplateNotificationAsync(templateParams, notificationRequest.Tags, token);
                }
                else
                {
                    var notificationTasks = notificationRequest.Tags
                        .Select((value, index) => (value, index))
                        .GroupBy(g => g.index / 20, i => i.value)
                        .Select(tags => _hub.SendTemplateNotificationAsync(templateParams, tags, token));

                    await Task.WhenAll(notificationTasks);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error sending notification");
                return false;
            }
        }
    }
}