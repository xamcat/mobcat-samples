using System;
using System.Globalization;
using System.Reflection;
using Microsoft.MobCAT;
using Microsoft.MobCAT.Forms.Services;
using Microsoft.MobCAT.MVVM.Abstractions;
using Weather.Localization;
using Weather.Services;
using Weather.Services.Abstractions;

namespace Weather
{
    public static class Bootstrap
    {
        public static void Begin(Action platformSpecificBegin = null)
        {
            var navigationService = new NavigationService();
            navigationService.RegisterViewModels(typeof(MainPage).GetTypeInfo().Assembly);

            ServiceContainer.Register<INavigationService>(navigationService);
            ServiceContainer.Register<IForecastsService>(() => new ForecastsService(ServiceConfig.WeatherServiceUrl, ServiceConfig.WeatherServiceApiKey));
            ServiceContainer.Register<IImageService>(() => new ImageService(ServiceConfig.WeatherServiceUrl, ServiceConfig.WeatherServiceApiKey));
            ServiceContainer.Register<IMainThreadAsyncService>(() => new MainThreadAsyncService());
            ServiceContainer.Register<IGeolocationService>(() => new GeolocationService());
            ServiceContainer.Register<IGeocodingService>(() => new GeocodingService());
            ServiceContainer.Register<ITimeOfDayImageService>(() => new TimeOfDayImageService());
            ServiceContainer.Register<IValueCacheService>(() => new ValueCacheService());

            //Log to AppCenter for release
#if RELEASE
            Logger.RegisterService(new AppCenterLoggingService());
#endif

            platformSpecificBegin?.Invoke();
        }
    }
}