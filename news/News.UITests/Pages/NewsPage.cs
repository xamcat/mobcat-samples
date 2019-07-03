using System;
using System.Collections.Generic;
using System.Linq;
using News.Helpers;
using NUnit.Framework;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace News.UITests.Pages
{
    public class NewsPage : BasePage
    {
        protected readonly Func<int, Query> FavoriteButton;
        public List<string> SupportedCategories => Constants.Categories.All;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(nameof(NewsPage)),
            iOS = x => x.Marked(nameof(NewsPage))
        };

        public NewsPage()
        {
            FavoriteButton = index => x => x.Marked(nameof(FavoriteButton)).Index(index);
        }

        public NewsPage AddFavorite(int articleIndex)
        {
            app.WaitForElement(FavoriteButton(articleIndex));
            app.Tap(FavoriteButton(articleIndex));
            app.Screenshot($"Added favorite article at index: {articleIndex}");
            return this;
        }

        public NewsPage ShowNextCategory()
        {
            app.SwipeRightToLeft(swipeSpeed: 1000);
            app.Screenshot("Swiped to the next available category");
            return this;
        }

        public NewsPage ValidateCategory(string category)
        {
            app.WaitForElement(category);
            Assert.NotNull(app.Query(x => x.Marked(category)));

            app.Screenshot($"Validated category ${category}");
            return this;
        }
    }
}