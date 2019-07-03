using System;
using System.Collections.Generic;

namespace News.Helpers
{
    public static class Constants
    {
        public const int DefaultArticlesPageSize = 15;

        public static class Style
        {
            //Theme Color Keys
            public const string BackgroundColorKey = "backgroundColor";
            public const string ArticleBackgroundColorKey = "articleBackgroundColor";
            public const string SeparatorColorKey = "separatorColor";
            public const string NavBarColorKey = "navBarColor";
            public const string EntryColorKey = "entryColor";
            public const string TextColorKey = "textColor";
            public const string AccentTextColorKey = "accentTextColor";

            //Dark Theme
            public const string BackgroundDarkThemeColor = "#585858";
            public const string ArticleBackgroundDarkThemeColor = "#E6272727";
            public const string SeparatorDarkThemeColor = "#2C2C2C";
            public const string NavBarDarkThemeColor = "#3C3C3C";
            public const string EntryDarkThemeColor = "#585858";
            public const string TextDarkThemeColor = "#FFFFFF";
            public const string AccentTextDarkThemeColor = "#D8D8D8";

            //Light Theme
            public const string BackgroundLightThemeColor = "#E2E2E2";
            public const string ArticleBackgroundLightThemeColor = "#E6D8D8D8";
            public const string SeparatorLightThemeColor = "#D3D3D3";
            public const string NavBarLightThemeColor = "#C3C3C3";
            public const string EntryLightThemeColor = "#A7A7A7";
            public const string TextLightThemeColor = "#000000";
            public const string AccentTextLightThemeColor = "#272727";
        }

        public static class Categories
        {
            public const string TopNews = "Top News \ud83d\udd25";
            public const string Business = "Business \ud83d\udcbc";
            public const string Entertainment = "Entertainment \ud83d\udc83\ud83d\udd7a";
            public const string Health = "Health \ud83e\uddec";
            public const string Science = "Science \ud83d\udd2c";
            public const string Sports = "Sports \ud83e\udd3c";
            public const string Technology = "Technology \ud83d\udc69‍\ud83d\udcbb";

            public static List<string> All { get; } = new List<string>
            {
                TopNews,
                Business,
                Entertainment,
                Health,
                Science,
                Sports,
                Technology,
            };
        }

        public static class Sources
        {
            public const string CNN = "CNN";
            public const string TheNewYorkTimes = "The New York Times";
            public const string ABCNews = "ABC News";
            public const string TheWashingtonPost = "The Washington Post";
            public const string FoxNews = "Fox News";
            public const string CBSNews = "CBS News";
            public const string NBCNews = "NBC News";
            public const string Reuters = "Reuters";
            public const string USAToday = "USA Today";
            public const string Bloomberg = "Bloomberg";
            public const string Wired = "Wired";
            public const string CNBC = "CNBC";

            public static List<string> All { get; } = new List<string>
            {
                CNN,
                TheNewYorkTimes,
                ABCNews,
                TheWashingtonPost,
                FoxNews,
                CBSNews,
                NBCNews,
                Reuters,
                USAToday,
                Bloomberg,
                Wired,
                CNBC,
            };
        }
    }
}