using System.Linq;
using System.Threading.Tasks;
using News.Helpers;
using News.Models;

namespace News.ViewModels
{
    /// <summary>
    /// Search view model to handle search results.
    /// </summary>
    public class SearchViewModel : BaseNewsViewModel
    {
        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { RaiseAndUpdate(ref _searchTerm, value); }
        }

        public SearchViewModel()
        {
        }

        protected async override Task<FetchArticlesResult> FetchArticlesAsync(int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            var result = new FetchArticlesResult(pageNumber, pageSize);
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return result;

            System.Diagnostics.Debug.WriteLine($"{GetType().Name} FetchArticlesAsync for [{SearchTerm}] Search Term");
            var articles = await NewsDataService.FetchArticlesBySearchQuery(SearchTerm);
            if (articles?.Articles != null)
            {
                result.Articles = articles.Articles.Select(a => new ArticleViewModel(a)).ToList();
                result.TotalCount = articles.TotalCount;
            }

            return result;
        }
    }
}