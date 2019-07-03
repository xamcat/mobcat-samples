using System;
using System.Threading;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Services.Abstractions;

namespace Weather.Services
{
    public class ForecastsService
        : BaseWeatherService, IForecastsService
    {
        public ForecastsService(string apiBaseAddress, string apiKey)
            : base(apiBaseAddress, apiKey)
        { }

        public Task<Forecast> GetForecastAsync(string city, TemperatureUnit unit = TemperatureUnit.Metric, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException(nameof(city));

            return GetAsync<Forecast>($"forecasts/{city}?units={unit.ToString()}", cancellationToken, SetApiKeyHeader);
        }

        public Task<Forecast> GetForecastAsync(double latitude, double longitude, TemperatureUnit unit = TemperatureUnit.Metric, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<Forecast>($"forecasts/coordinates?latitude={latitude.ToString()}&longitude={longitude.ToString()}&units={unit.ToString()}", cancellationToken, SetApiKeyHeader);
        }
    }
}