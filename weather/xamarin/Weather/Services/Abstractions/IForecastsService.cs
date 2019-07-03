using System.Threading;
using System.Threading.Tasks;
using Weather.Models;

namespace Weather.Services.Abstractions
{
    public interface IForecastsService
    {
        Task<Forecast> GetForecastAsync(string city, TemperatureUnit unit = TemperatureUnit.Metric, CancellationToken cancellationToken = default(CancellationToken));

        Task<Forecast> GetForecastAsync(double latitude, double longitude, TemperatureUnit unit = TemperatureUnit.Metric, CancellationToken cancellationToken = default(CancellationToken));
    }
}