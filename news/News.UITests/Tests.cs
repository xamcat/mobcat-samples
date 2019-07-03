using System;
using System.IO;
using System.Linq;
using News.UITests.Pages;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace News.UITests
{
    public class Tests : BaseTestFixture
    {
        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void ViewEachTabInDarkModeAndLightMode()
        {

            new NewsPage()
                .SelectTabOption(BasePage.NavigationTabOption.Sources);

            new SourcesPage()
                .SelectTabOption(BasePage.NavigationTabOption.Favorites);

            new FavoritesPage()
                .SelectTabOption(BasePage.NavigationTabOption.Search);

            new SearchPage()
                .SelectTabOption(BasePage.NavigationTabOption.Settings);

            new SettingsPage()
                .SetTheme(Themes.light)
                .SelectTabOption(BasePage.NavigationTabOption.News);

            new NewsPage()
               .SelectTabOption(BasePage.NavigationTabOption.Sources);

            new SourcesPage()
                .SelectTabOption(BasePage.NavigationTabOption.Favorites);

            new FavoritesPage()
                .SelectTabOption(BasePage.NavigationTabOption.Search);

            new SearchPage();
        }

        [Test]
        public void SearchForMicrosoftNews()
        {
            new NewsPage()
                .SelectTabOption(BasePage.NavigationTabOption.Search);

            new SearchPage()
                .Search("Microsoft");
        }

        [Test]
        public void AddAndRemoveFavorite()
        {
            new NewsPage()
                .AddFavorite(articleIndex: 0);

            new NewsPage()
                .SelectTabOption(BasePage.NavigationTabOption.Favorites);

            new FavoritesPage()
                .WaitToBecomeNotEmpty()
                .RemoveFavorite(favoriteIndex: 0)
                .WaitToBecomeNotEmpty()
                .RefreshFavorites()
                .WaitToBecomeEmpty();
        }

        [Test]
        public void SetLightMode()
        {
            new NewsPage()
                .SelectTabOption(BasePage.NavigationTabOption.Settings);

            new SettingsPage()
                .SetTheme(Themes.light);
        }

        [Test]
        public void SwipeThroughEachCategory()
        {
            foreach (var selectedCategory in new NewsPage().SupportedCategories)
            {
                new NewsPage()
                    .ValidateCategory(selectedCategory)
                    .ShowNextCategory();
            }
        }

        [Test]
        public void SwipeThroughEachSource()
        {
            new NewsPage()
                .SelectTabOption(BasePage.NavigationTabOption.Sources);

            foreach (var selectedSource in new SourcesPage().SupportedSources)
            {
                new SourcesPage()
                    .ValidateSource(selectedSource)
                    .ShowNextSource();
            }
        }

        [Test]
        [Ignore(reason:"Local only")]
        public void Repl()
        {
            if (TestEnvironment.IsTestCloud)
                Assert.Ignore("Local only");

            app.Repl();
        }
    }
}
