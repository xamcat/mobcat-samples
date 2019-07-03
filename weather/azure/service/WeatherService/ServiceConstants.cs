namespace WeatherService
{
    internal static class ServiceConstants
    {
        internal const string ApiKeySettingName = "API_KEY_NAME";
        internal const string AppInsightsSettingName = "APP_INSIGHTS_KEY_NAME";
        internal const string CacheConnectionStringSettingName = "CACHE_CONNECTION_STRING_KEY_NAME";
        internal const string CognitiveServicesSearchSettingName = "COGNITIVE_SERVICES_SEARCH_KEY_NAME";
        internal const string OpenWeatherMapAppIdSettingName = "OPEN_WEATHER_MAP_APP_ID_KEY_NAME";
        internal const string KeyVaultEndpointSettingName = "KEYVAULT_ENDPOINT";
        internal const string UseKeyVaultSettingName = "USE_KEYVAULT";
        internal const string OpenWeatherMapHttpClientIdentifier = "OpenWeatherMap";
        internal const string OpenWeatherMapApiBaseAddress = "http://api.openweathermap.org/data/2.5/weather";
        internal const string BingImagesSearchHttpClientIdentifier = "BingImages";
        internal const string BingImagesSearchApiBaseAddress = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";
        internal const string GeneralHttpClientIdentifier = "GeneralHttpClient";
        internal const string BingImagesSearchApiKeyHeaderName = "Ocp-Apim-Subscription-Key";
        internal const string CachePrefixForecasts = "forecasts";
        internal const string CachePrefixImages = "images";
    }
}