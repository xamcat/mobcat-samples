using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Services.Abstractions;
using Xamarin.Essentials;

namespace Weather.Services
{
    public class GeocodingService : IGeocodingService
    {
        public async Task<IEnumerable<Place>> GetPlacesAsync(Coordinates coordinates)
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(new Location(coordinates.Latitude, coordinates.Longitude));

            if (placemarks == null || !placemarks.Any())
            {
                return null;
            }

            return placemarks.Select(PlacemarkToPlace);
        }

        private Place PlacemarkToPlace(Placemark placemark)
        {
            var cityName = default(string);
            if (!string.IsNullOrEmpty(placemark.Locality))
            {
                cityName = placemark.Locality;
            }
            else
            {
                cityName = placemark.FeatureName;
            }

            return new Place { CityName = cityName };
        }
    }
}