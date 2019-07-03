using System;
using System.Threading.Tasks;
using News.Services.Abstractions;
using News.Services.FakeNews;
using Xamarin.Forms;
using News.Helpers;
using Newtonsoft.Json;
using News.Models;
using System.Linq;

#if DEBUG
[assembly: Dependency(typeof(FakeNewsDataService))]
#endif
namespace News.Services.FakeNews
{
    public class FakeNewsDataService : INewsDataService
    {
        public FakeNewsDataService()
        {
        }

        public async Task<FetchArticlesResponse> FetchArticlesByCategory(Categories? category = null, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesByCategory: categrory {category}, page {pageNumber}, pageSize {pageSize}");

            // Simulate network request
            await Task.Delay(1500);
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            var resourceName = $"{GetType().Namespace}.category.{category?.ToString().ToLower() ?? "all"}.json";
            var prestoredResponseContent = await resourceName.ReadResourceContent();
            if (!string.IsNullOrWhiteSpace(prestoredResponseContent))
            {
                var articles = JsonConvert.DeserializeObject<ArticlesResult>(prestoredResponseContent);
                if (articles?.Articles != null)
                {
                    
                    result.Articles = articles.Articles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    result.TotalCount = articles.Articles.Count;
                }
            }

            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesByCategory loaded {result.Articles.Count} (page {result.PageNumber} | size {result.PageSize}) out of total {result.TotalCount} items");

            return result;
        }

        public async Task<FetchArticlesResponse> FetchArticlesBySource(string source, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesBySource: source {source}, page {pageNumber}, pageSize {pageSize}");

            // Simulate network request
            await Task.Delay(1500);
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            var resourceName = $"{GetType().Namespace}.source.{source}.json";
            var prestoredResponseContent = await resourceName.ReadResourceContent();
            if (!string.IsNullOrWhiteSpace(prestoredResponseContent))
            {
                var articles = JsonConvert.DeserializeObject<ArticlesResult>(prestoredResponseContent);
                if (articles?.Articles != null)
                {
                    result.Articles = articles.Articles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    result.TotalCount = articles.Articles.Count;
                }
            }

            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesBySource loaded {result.Articles.Count} (page {result.PageNumber} | size {result.PageSize}) out of total {result.TotalCount} items");

            return result;
        }

        public async Task<FetchArticlesResponse> FetchArticlesBySearchQuery(string query, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            if (string.IsNullOrWhiteSpace(query))
                return result;

            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesBySearchQuery: query {query}, page {pageNumber}, pageSize {pageSize}");

            query = query.ToLower();
            if (query != "trump")
                query = "bitcoin";

            // Simulate network request
            await Task.Delay(1500);
            var resourceName = $"{GetType().Namespace}.search.{query}.json";
            var prestoredResponseContent = await resourceName.ReadResourceContent();
            if (!string.IsNullOrWhiteSpace(prestoredResponseContent))
            {
                var articles = JsonConvert.DeserializeObject<ArticlesResult>(prestoredResponseContent);
                if (articles?.Articles != null)
                {
                    result.Articles = articles.Articles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    result.TotalCount = articles.Articles.Count;
                }
            }

            System.Diagnostics.Debug.WriteLine($"FakeNewsDataService.FetchArticlesBySearchQuery loaded {result.Articles.Count} (page {result.PageNumber} | size {result.PageSize}) out of total {result.TotalCount} items");

            return result;
        }
    }
}
