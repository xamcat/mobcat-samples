using System;
using System.Linq;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace News.UITests.Pages
{
    public class FavoritesPage : BasePage
    {
        protected readonly Func<int,Query> FavoriteCell;
        protected readonly Query NewsInfiniteListView;
        protected readonly Func<int, Query> FavoriteButton;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(nameof(FavoritesPage)),
            iOS = x => x.Marked(nameof(FavoritesPage))
        };

        public FavoritesPage()
        {
            FavoriteButton = index => x => x.Marked(nameof(FavoriteButton)).Index(index);

            if(OnAndroid)
            {
                NewsInfiniteListView = x => x.Class("ListView");
                FavoriteCell = index => x => x.Class("ViewCellRenderer_ViewCellContainer").Index(index);
            }
            if(OniOS)
            {
                NewsInfiniteListView = x => x.Class("UITableView");
                FavoriteCell = index => x => x.Class("Xamarin_Forms_Platform_iOS_ViewCellRenderer_ViewTableCell").Index(index);
            }
        }

        public FavoritesPage WaitToBecomeEmpty()
        {
            app.Screenshot("Wait to become empty");
            app.WaitForNoElement(FavoriteCell(0));
            app.Screenshot("Favorites are empty");
            return this;
        }

        public FavoritesPage WaitToBecomeNotEmpty()
        {
            app.Screenshot("Wait to become not empty");
            app.WaitForElement(FavoriteCell(0));
            app.Screenshot("Favorites are not empty");
            return this;
        }

        public FavoritesPage RefreshFavorites()
        {
            app.WaitForElement(NewsInfiniteListView);
            var newsList = app.Query(NewsInfiniteListView).First();
            app.DragCoordinates(newsList.Rect.X, newsList.Rect.Y, newsList.Rect.X, newsList.Rect.CenterY);
            return this;
        }

        public FavoritesPage RemoveFavorite(int favoriteIndex)
        {
            app.WaitForElement(FavoriteButton(favoriteIndex));
            app.Tap(FavoriteButton(favoriteIndex));
            app.Screenshot("Removed favorite article");
            return this;
        }
    }
}
