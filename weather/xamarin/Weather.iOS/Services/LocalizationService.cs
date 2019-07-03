using System;
using System.Globalization;
using System.Threading;
using Foundation;
using Weather.Localization;
using Weather.Services;
using Weather.Services.Abstractions;

namespace Weather.iOS.Services
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
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = iOSToDotnetLanguage(pref);
            }

            return GetCultureInfo(netLanguage);
        }

        private string iOSToDotnetLanguage(string iOSLanguage)
        {
            Console.WriteLine($"iOS Language: {iOSLanguage}");
            var netLanguage = iOSLanguage;

            //certain languages need to be converted to CultureInfo equivalent
            switch (iOSLanguage)
            {
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture
                    netLanguage = "ms"; // closest supported
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
                case "pt":
                    netLanguage = "pt-PT"; // fallback to Portuguese (Portugal)
                    break;
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
