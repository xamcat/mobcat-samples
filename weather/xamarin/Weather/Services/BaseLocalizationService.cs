using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.MobCAT;
using Weather.Localization;
using Weather.Services.Abstractions;

namespace Weather.Services
{
    public abstract class BaseLocalizationService : ILocalizationService
    {
        const string ResourceId = "Weather.Localization.AppResources";
        static readonly Lazy<ResourceManager> ResourceManager = new Lazy<ResourceManager>(() => new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(BaseLocalizationService)).Assembly));
        Dictionary<string, CultureInfo> _cultureInfoCache = new Dictionary<string, CultureInfo>();

        public abstract CultureInfo GetCurrentCultureInfo();

        public abstract void SetLocale(CultureInfo cultureInfo);

        public string Translate(string resourceName)
        {
            try
            {
                var cultureInfo = GetCurrentCultureInfo();
                var translation = ResourceManager.Value.GetString(resourceName, cultureInfo);
                return translation;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return string.Empty;
        }

        protected CultureInfo GetCultureInfo(string netLanguage)
        {
            if (_cultureInfoCache.ContainsKey(netLanguage))
            {
                return _cultureInfoCache[netLanguage];
            }

            var cultureInfo = default(CultureInfo);
            try
            {
                cultureInfo = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException exception1)
            {
                Logger.Error(exception1);
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
                // fallback to first characters, in this case "en"
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    Console.WriteLine($"{netLanguage} failed, trying {fallback} ({exception1.Message})");
                    cultureInfo = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException exception2)
                {
                    Logger.Error(exception2);
                    // iOS language not valid .NET culture, falling back to English
                    Console.WriteLine($"{netLanguage} couldn't be set, using 'en' ({exception2.Message})");
                    cultureInfo = new CultureInfo("en");
                }
            }

            _cultureInfoCache.Add(netLanguage, cultureInfo);
            return cultureInfo;
        }

        protected abstract string ToDotnetFallbackLanguage(PlatformCulture platformCulture);
    }
}