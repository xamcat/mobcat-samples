using System;
namespace Weather
{
    public class Constants
    {
        public class CacheKeys
        {
            public const string CacheSavedDateTimeKey = "CacheSavedDateTimeKey";
            public const string CityNameKey = "CityNameKey";
            public const string CurrentTempKey = "CurrentTempKey";
            public const string HighTempKey = "HighTempKey";
            public const string LowTempKey = "LowTempKey";
            public const string WeatherDescriptionKey = "WeatherDescriptionKey";
            public const string WeatherImageKey = "WeatherImageKey";
            public const string IsCelsiusKey = "IsCelsiusKey";
            public const string WeatherIconKey = "WeatherIconKey";
        }

        public class LanguageResourceKeys
        {
            public const string WeatherUnknownKey = "unknownweather";
        }
    }
}
