using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MobCAT;
using Moq;
using Weather.Models;
using Weather.Services.Abstractions;
using Weather.ViewModels;
using Xunit;

namespace Weather.UnitTests
{
    public class UnitTest
    {
        [Fact]
        public async Task Test1Async()
        {
            //Mock the forecast service
            var mockForecastService = new Mock<IForecastsService>();
            mockForecastService.Setup(a => a.GetForecastAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<TemperatureUnit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(
                new Forecast
                {
                    Id = "MockForecast",
                    CurrentTemperature = "900",
                    Description = "It's way too hot",
                    MinTemperature = "800",
                    MaxTemperature = "1000",
                    Name = "Forecast Name",
                    Overview = "Forecast Overview"
                }));

            ServiceContainer.Register<IForecastsService>(mockForecastService.Object);

            //Mock the image service
            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(a => a.GetImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult("Image URL"));

            ServiceContainer.Register<IImageService>(mockImageService.Object);

            //Mock geolocation
            var mockGeolocationService = new Mock<IGeolocationService>();
            mockGeolocationService.Setup(a => a.GetLastKnownLocationAsync())
                .Returns(Task.FromResult(new Coordinates()));
            mockGeolocationService.Setup(a => a.GetLocationAsync())
                .Returns(Task.FromResult(new Coordinates()));

            ServiceContainer.Register<IGeolocationService>(mockGeolocationService.Object);

            //Mock geocoding service
            var mockGeocodingService = new Mock<IGeocodingService>();
            mockGeocodingService.Setup(a => a.GetPlacesAsync(It.IsAny<Coordinates>()))
                .Returns(Task.FromResult(new List<Place>{ new Place {
                CityName = "MobCAT City"
                }}.AsEnumerable()));

            ServiceContainer.Register<IGeocodingService>(mockGeocodingService.Object);

            //Mock the value cache service
            var mockValueCacheService = new Mock<IValueCacheService>();
            ServiceContainer.Register<IValueCacheService>(mockValueCacheService.Object);

            //Mock the localization service
            var mockLocalizationService = new Mock<ILocalizationService>();
            mockLocalizationService.Setup(a => a.Translate(It.IsAny<string>()))
                .Returns<string>(x => x); //Just return what was passed in
            ServiceContainer.Register<ILocalizationService>(mockLocalizationService.Object);

            //Init the VM
            var weatherViewModel = new WeatherViewModel();
            await weatherViewModel.InitAsync();

            //Get expecteds for asserts
            var expectedImage = await mockImageService.Object.GetImageAsync(default(string), default(String), default(CancellationToken));
            var expectedForecast = await mockForecastService.Object.GetForecastAsync(default(double), default(double), default(TemperatureUnit), default(CancellationToken));
            var expectedPlaces = await mockGeocodingService.Object.GetPlacesAsync(default(Coordinates));
            var expectedPlaceName = expectedPlaces?.FirstOrDefault()?.CityName;

            var weatherDescriptionTranslationResourceKey = expectedForecast.Overview.Trim().Replace(" ", "").ToLower();

            //Assert
            Assert.Equal(weatherDescriptionTranslationResourceKey, weatherViewModel.WeatherDescription);
            Assert.Equal(expectedForecast.CurrentTemperature, weatherViewModel.CurrentTemp);
            Assert.Equal(expectedForecast.MaxTemperature, weatherViewModel.HighTemp);
            Assert.Equal(expectedForecast.MinTemperature, weatherViewModel.LowTemp);
            Assert.Equal(expectedImage, weatherViewModel.WeatherImage);
            Assert.Equal(expectedPlaceName, weatherViewModel.CityName);
        }
    }
}
