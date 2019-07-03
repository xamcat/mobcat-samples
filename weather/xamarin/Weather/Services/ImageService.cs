using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MobCAT.Services;
using Weather.Models;
using Weather.Services.Abstractions;

namespace Weather.Services
{
    public class ImageService
        : BaseWeatherService, IImageService
    {
        public ImageService(string apiBaseAddress, string apiKey)
            : base(apiBaseAddress, apiKey)
        { }

        public async Task<string> GetImageAsync(string city, string weather, CancellationToken cancellationToken = default(CancellationToken))
        {
            var searchWeather = weather.Replace(" ", "+");
            var remoteImage = await GetAsync<CityWeatherImage>($"images/{city}?weather={searchWeather}", cancellationToken, SetApiKeyHeader).ConfigureAwait(false);
            var intermediaryDownloadUrl = $"{BaseApiUrl}images/downloads?imageUrl={remoteImage.ImageUrl.Split('?').FirstOrDefault()}&key={ApiKey}";

            return await WebImageCache.RetrieveImage(intermediaryDownloadUrl, $"{city}-{searchWeather}");
        }
    }
}