using System.Threading.Tasks;
using News.Helpers;
using News.Models;

namespace News.Services.Abstractions
{
    public interface INewsDataService
    {
        Task<FetchArticlesResponse> FetchArticlesByCategory(Categories? category = null, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize);
        Task<FetchArticlesResponse> FetchArticlesBySource(string source, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize);
        Task<FetchArticlesResponse> FetchArticlesBySearchQuery(string query, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize);
    }
}