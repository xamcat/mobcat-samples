using System.Linq;
using System.Threading.Tasks;
using News.Helpers;
using News.Models;
using News.Services.Abstractions;
using Xamarin.Forms;

namespace News.ViewModels
{
    /// <summary>
    /// Favorite news view model.
    /// </summary>
    public class FavoritesViewModel : BaseNewsViewModel
    {
        public FavoritesViewModel()
        {

        }

        protected override Task<FetchArticlesResult> FetchArticlesAsync(int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            var result = new FetchArticlesResult(pageNumber, pageSize);
            var favoritesService = DependencyService.Resolve<IFavoritesService>();
            var favorites = favoritesService.Get()
                .Select(f => new ArticleViewModel(f))
                .ToList();

            if (favorites != null)
            {
                result.Articles = favorites;
                result.TotalCount = favorites.Count;
            };

            return Task.FromResult(result);
        }
    }
}