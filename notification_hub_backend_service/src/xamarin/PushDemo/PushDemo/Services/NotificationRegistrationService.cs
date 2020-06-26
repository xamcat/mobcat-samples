using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PushDemo.Models;
using Xamarin.Essentials;

namespace PushDemo.Services
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        const string CachedTagsKey = "cached_tags";
        const string RequestUrl = "api/notifications/installations";

        string _baseApiUrl;
        HttpClient _client;
        IDeviceInstallationService _deviceInstallationService;

        public NotificationRegistrationService(string baseApiUri, string apiKey)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("apikey", apiKey);

            _baseApiUrl = baseApiUri;
        }

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService = ServiceContainer.Resolve<IDeviceInstallationService>());

        public Task DeregisterDeviceAsync()
        {
            var deviceId = DeviceInstallationService?.GetDeviceId();

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new Exception("Unable to resolve an ID for the device.");

            SecureStorage.Remove(CachedTagsKey);

            return SendAsync(HttpMethod.Delete, $"{RequestUrl}/{deviceId}");
        }

        public async Task RegisterDeviceAsync(params string[] tags)
        {
            var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation(tags);

            await SendAsync<DeviceInstallation>(HttpMethod.Put, RequestUrl, deviceInstallation)
                .ConfigureAwait(false);

            var serializedTags = JsonConvert.SerializeObject(tags);
            await SecureStorage.SetAsync(CachedTagsKey, serializedTags);
        }

        public async Task RefreshRegistrationAsync()
        {
            var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedTags))
                return;

            var tags = JsonConvert.DeserializeObject<string[]>(serializedTags);

            await RegisterDeviceAsync(tags);
        }

        async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
        {
            string serializedContent = null;

            await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj))
                .ConfigureAwait(false);            

            await SendAsync(requestType, requestUri, serializedContent);
        }

        async Task SendAsync(
            HttpMethod requestType,
            string requestUri,
            string jsonRequest = null)
        {
            var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

            if (jsonRequest != null)
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}