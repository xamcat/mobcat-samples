using System;
using System.Threading.Tasks;
using Microsoft.MobCAT;
using Weather.Models;
using Weather.Services.Abstractions;
using Xamarin.Essentials;

namespace Weather.Services
{
    public class GeolocationService : IGeolocationService
    {
        readonly Lazy<IMainThreadAsyncService> mainThreadAsyncService = new Lazy<IMainThreadAsyncService>(() => ServiceContainer.Resolve<IMainThreadAsyncService>());

        public async Task<Coordinates> GetLastKnownLocationAsync()
        {
            var location = await mainThreadAsyncService.Value.RunOnMainThreadAsync(Geolocation.GetLastKnownLocationAsync);
            return location == null ? null : LocationToCoordinates(location);
        }

        async Task<Coordinates> IGeolocationService.GetLocationAsync()
        {
            var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await mainThreadAsyncService.Value.RunOnMainThreadAsync(async () => await Geolocation.GetLocationAsync(geolocationRequest));
            return location == null ? null : LocationToCoordinates(location);
        }

        private Coordinates LocationToCoordinates(Location location)
        {
            return new Coordinates
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}
