using System;
using NUnit.Framework;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace News.UITests
{
    public abstract class BasePage
    {
        protected IApp app => AppManager.App;
        protected bool OnAndroid => AppManager.Platform == Platform.Android;
        protected bool OniOS => AppManager.Platform == Platform.iOS;

        protected abstract PlatformQuery Trait { get; }

        protected BasePage()
        {
            AssertOnPage(TimeSpan.FromSeconds(30));
            app.Screenshot("On " + this.GetType().Name);
        }

        /// <summary>
        /// Verifies that the trait is still present. Defaults to no wait.
        /// </summary>
        /// <param name="timeout">Time to wait before the assertion fails</param>
        protected void AssertOnPage(TimeSpan? timeout = default(TimeSpan?))
        {
            var message = "Unable to verify on page: " + this.GetType().Name;

            if (timeout == null)
                Assert.IsNotEmpty(app.Query(Trait.Current), message);
            else
                Assert.DoesNotThrow(() => app.WaitForElement(Trait.Current, timeout: timeout), message);
        }

        /// <summary>
        /// Verifies that the trait is no longer present. Defaults to a 5 second wait.
        /// </summary>
        /// <param name="timeout">Time to wait before the assertion fails</param>
        protected void WaitForPageToLeave(TimeSpan? timeout = default(TimeSpan?))
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var message = "Unable to verify *not* on page: " + this.GetType().Name;

            Assert.DoesNotThrow(() => app.WaitForNoElement(Trait.Current, timeout: timeout), message);
        }

        // You can edit this file to define functionality that is common across many or all pages in your app.
        // For example, you could add a method here to open a side menu that is accesible from all pages.
        // To keep things more organized, consider subclassing BasePage and including common page actions there.
        // For some examples check out https://github.com/xamarin-automation-service/uitest-pop-example/wiki

        public void SelectTabOption(NavigationTabOption item)
        {
            app.Screenshot($"Selecting Tab {item.Current.GetType().Name}");
            app.Tap(item.Current);
        }

        public class NavigationTabOption : PlatformQuery
        {
            public static readonly NavigationTabOption News = new NavigationTabOption
            {
                Android = x => x.Marked("news"),
                iOS = x => x.Marked("news")
            };

            public static readonly NavigationTabOption Sources = new NavigationTabOption
            {
                Android = x => x.Marked("sources"),
                iOS = x => x.Marked("sources")
            };

            public static readonly NavigationTabOption Favorites = new NavigationTabOption
            {
                Android = x => x.Marked("favorites"),
                iOS = x => x.Marked("favorites")
            };

            public static readonly NavigationTabOption Search = new NavigationTabOption
            {
                Android = x => x.Marked("search"),
                iOS = x => x.Marked("search")
            };

            public static readonly NavigationTabOption Settings = new NavigationTabOption
            {
                Android = x => x.Marked("settings"),
                iOS = x => x.Marked("settings")
            };
        }
    }
}
