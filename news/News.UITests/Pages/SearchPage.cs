using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace News.UITests.Pages
{
    public class SearchPage : BasePage
    {
        protected readonly Query FindButton;
        protected readonly Query SearchEntry;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(nameof(SearchPage)),
            iOS = x => x.Marked(nameof(SearchPage))
        };

        public SearchPage()
        {
            SearchEntry = x => x.Marked(nameof(SearchEntry));
            FindButton = x => x.Marked(nameof(FindButton));
        }

        public SearchPage Search(string query)
        {
            app.WaitForElement(SearchEntry);
            app.Tap(SearchEntry);
            app.EnterText(query);
            app.PressEnter();
            app.WaitForElement(FindButton);
            app.Tap(FindButton);
            app.Screenshot($"Searched for {query}");
            return this;
        }
    }
}
