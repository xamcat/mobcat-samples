using System;
using System.Collections.Generic;
using News.Helpers;
using News.Services;
using News.Services.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(VisualThemeService))]
namespace News.Services
{
    public class VisualThemeService : IVisualThemeService
    {
        //Populate themes here
        private readonly Dictionary<VisualTheme, Dictionary<string, string>> _visualThemes =
            new Dictionary<VisualTheme, Dictionary<string, string>>
            {
               { VisualTheme.Light, new Dictionary<string, string>
                    {
                        {Constants.Style.BackgroundColorKey, Constants.Style.BackgroundLightThemeColor },
                        {Constants.Style.ArticleBackgroundColorKey, Constants.Style.ArticleBackgroundLightThemeColor },
                        {Constants.Style.SeparatorColorKey, Constants.Style.SeparatorLightThemeColor },
                        {Constants.Style.NavBarColorKey, Constants.Style.NavBarLightThemeColor },
                        {Constants.Style.EntryColorKey, Constants.Style.EntryLightThemeColor },
                        {Constants.Style.TextColorKey, Constants.Style.TextLightThemeColor },
                        {Constants.Style.AccentTextColorKey, Constants.Style.AccentTextLightThemeColor }
                    }},
                { VisualTheme.Dark, new Dictionary<string, string>
                    {
                        {Constants.Style.BackgroundColorKey, Constants.Style.BackgroundDarkThemeColor },
                        {Constants.Style.ArticleBackgroundColorKey, Constants.Style.ArticleBackgroundDarkThemeColor },
                        {Constants.Style.SeparatorColorKey, Constants.Style.SeparatorDarkThemeColor },
                        {Constants.Style.NavBarColorKey, Constants.Style.NavBarDarkThemeColor },
                        {Constants.Style.EntryColorKey, Constants.Style.EntryDarkThemeColor },
                        {Constants.Style.TextColorKey, Constants.Style.TextDarkThemeColor },
                        {Constants.Style.AccentTextColorKey, Constants.Style.AccentTextDarkThemeColor }
                    }}
            };

        public VisualTheme CurrentVisualTheme
        {
            get
            {
                var selectedTheme = Preferences.Get(nameof(VisualTheme), defaultValue: nameof(VisualTheme.Dark));
                Enum.TryParse<VisualTheme>(selectedTheme, out var theme);
                return theme;
            }
        }


        public void LoadSelectedVisualTheme()
        {
            var selectedTheme = Preferences.Get(nameof(VisualTheme), defaultValue: nameof(VisualTheme.Dark));
            Enum.TryParse<VisualTheme>(selectedTheme, out var theme);
            SetVisualTheme(theme);
        }

        public void SetVisualTheme(VisualTheme theme)
        {
            var themeColors = _visualThemes[theme];

            Application.Current.Resources[Constants.Style.BackgroundColorKey] = themeColors[Constants.Style.BackgroundColorKey].ToColor();
            Application.Current.Resources[Constants.Style.ArticleBackgroundColorKey] = themeColors[Constants.Style.ArticleBackgroundColorKey].ToColor();
            Application.Current.Resources[Constants.Style.TextColorKey] = themeColors[Constants.Style.TextColorKey].ToColor();
            Application.Current.Resources[Constants.Style.AccentTextColorKey] = themeColors[Constants.Style.AccentTextColorKey].ToColor();
            Application.Current.Resources[Constants.Style.NavBarColorKey] = themeColors[Constants.Style.NavBarColorKey].ToColor();
            Application.Current.Resources[Constants.Style.SeparatorColorKey] = themeColors[Constants.Style.SeparatorColorKey].ToColor();
            Application.Current.Resources[Constants.Style.EntryColorKey] = themeColors[Constants.Style.EntryColorKey].ToColor();

            Preferences.Set(nameof(VisualTheme), value: theme.ToString());
        }
    }
}
