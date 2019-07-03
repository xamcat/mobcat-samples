using System.Linq;
using System.Threading.Tasks;
using News.Models;
using News.Helpers;

namespace News.ViewModels
{
    /// <summary>
    /// Category news view model.
    /// </summary>
    public class CategoryNewsViewModel : BaseNewsViewModel
    {
        public string Title { get; }

        public Categories? Category { get; }

        public CategoryNewsViewModel(string title, Categories? category = null)
        {
            Title = title;
            Category = category;
        }

        protected async override Task<FetchArticlesResult> FetchArticlesAsync(int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().Name} FetchArticlesAsync for {Title} Category");
            var articles = await NewsDataService.FetchArticlesByCategory(Category, pageNumber, pageSize);
            var result = new FetchArticlesResult(pageNumber, pageSize)
            {
                Articles = articles.Articles.Select(a => new ArticleViewModel(a)).ToList(),
                TotalCount = articles.TotalCount,
            };

            return result;
        }
    }
}