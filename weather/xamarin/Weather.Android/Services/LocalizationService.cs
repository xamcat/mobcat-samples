using System;
using System.Globalization;
using System.Threading;
using Weather.Localization;
using Weather.Services;
using Weather.Services.Abstractions;

namespace Weather.Droid.Services
{
    public class LocalizationService : BaseLocalizationService
    {
        public override void SetLocale(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            Console.WriteLine($"CurrentCulture set: {cultureInfo.Name}");
        }

        public override CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var androidLocale = Java.Util.Locale.Default;
            netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));

            return GetCultureInfo(netLanguage);
        }

        private string AndroidToDotnetLanguage(string androidLanguage)
        {
            Console.WriteLine($"Android Language: {androidLanguage}");
            var netLanguage = androidLanguage;

            //certain languages need to be converted to CultureInfo equivalent
            switch (androidLanguage)
            {
                case "ms-BN":   // "Malaysian (Brunei)" not supported .NET culture
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture
                    netLanguage = "ms"; // closest supported
                    break;
                case "in-ID":  // "Indonesian (Indonesia)" has different code in  .NET 
                    netLanguage = "id-ID"; // correct code for .NET
                    break;
                case "gsw-CH":  // "Schwiizertüütsch (Swiss German)" not supported .NET culture
                    netLanguage = "de-CH"; // closest supported
                    break;
                    // add more application-specific cases here (if required)
                    // ONLY use cultures that have been tested and known to work
            }

            Console.WriteLine($".NET Language/Locale: {netLanguage}");
            return netLanguage;
        }

        protected override string ToDotnetFallbackLanguage(PlatformCulture platformCulture)
        {
            Console.WriteLine($".NET Fallback Language: {platformCulture.LanguageCode}");
            var netLanguage = platformCulture.LanguageCode; // use the first part of the identifier (two chars, usually);

            switch (platformCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
                    break;
                    // add more application-specific cases here (if required)
                    // ONLY use cultures that have been tested and known to work
            }

            Console.WriteLine($".NET Fallback Language/Locale: {netLanguage} (application-specific)");
            return netLanguage;
        }
    }
}
