using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using News.Models;
using News.Helpers;

namespace News.ViewModels
{
    /// <summary>
    /// Source news view model.
    /// </summary>
    public class SourceNewsViewModel : BaseNewsViewModel
    {
        public string Title { get; }

        public string Source { get; }

        public SourceNewsViewModel(string title, string source)
        {
            Title = title;
            Source = source;
        }

        protected async override Task<FetchArticlesResult> FetchArticlesAsync(int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().Name} FetchArticlesAsync for {Title} Source");
            var result = new FetchArticlesResult(pageNumber, pageSize);
            var articles = await NewsDataService.FetchArticlesBySource(Source);
            if (articles?.Articles != null)
            {
                result.Articles = articles.Articles.Select(a => new ArticleViewModel(a)).ToList();
                result.TotalCount = articles.TotalCount;
            }
            return result;
        }
    }
}