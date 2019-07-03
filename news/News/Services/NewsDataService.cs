using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MobCAT.Services;
using News.Helpers;
using News.Models;
using News.Services;
using News.Services.Abstractions;
using Xamarin.Forms;

#if !DEBUG
[assembly: Dependency(typeof(NewsDataService))]
#endif
namespace News.Services
{
    public class NewsDataService : BaseHttpService, INewsDataService
    {
        public NewsDataService()
            : base(ServiceConfig.NewsServiceUrl, null)
        {
            Serializer = new NewtonsoftJsonSerializer();
        }

        public async Task<FetchArticlesResponse> FetchArticlesByCategory(Categories? category = null, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            var request = new ArticlesRequest
            {
                Country = Countries.US,
                Language = Languages.EN,
                Category = category,
                Page = pageNumber,
                PageSize = pageSize,
            };

            var actionUrl = ToActionRelativeUrl("top-headlines", request);
            var articles = await GetAsync<ArticlesResult>(actionUrl);
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            if (articles?.Articles != null)
            {
                result.Articles = articles.Articles;
                result.TotalCount = articles.TotalResults;
            }

            return result;
        }

        public async Task<FetchArticlesResponse> FetchArticlesBySource(string source, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            var request = new ArticlesRequest
            {
                Sources = new List<string> { source },
                Language = Languages.EN,
                Page = pageNumber,
                PageSize = pageSize,
            };

            var actionUrl = ToActionRelativeUrl("top-headlines", request);
            var articles = await GetAsync<ArticlesResult>(actionUrl);
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            if (articles?.Articles != null)
            {
                result.Articles = articles.Articles;
                result.TotalCount = articles.TotalResults;
            }

            return result;
        }

        public async Task<FetchArticlesResponse> FetchArticlesBySearchQuery(string query, int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize)
        {
            var result = new FetchArticlesResponse(pageNumber, pageSize);
            if (string.IsNullOrWhiteSpace(query))
                return result;

            var request = new ArticlesRequest
            {
                Query = query,
                Country = Countries.US,
                Language = Languages.EN,
                Page = pageNumber,
                PageSize = pageSize,
            };

            var actionUrl = ToActionRelativeUrl("top-headlines", request);
            var articles = await GetAsync<ArticlesResult>(actionUrl);
            if (articles?.Articles != null)
            {
                result.Articles = articles.Articles;
                result.TotalCount = articles.TotalResults;
            }

            return result;
        }

        private string ToActionRelativeUrl(string action, ArticlesRequest requestParams = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentNullException(nameof(action));

            var actionUrlBuilder = new StringBuilder($"/{action}?");
            if (requestParams != null)
            {
                if (!string.IsNullOrWhiteSpace(requestParams.Query))
                {
                    actionUrlBuilder.Append($"q={requestParams.Query}&");
                }

                if (requestParams.Country != null)
                {
                    actionUrlBuilder.Append($"country={requestParams.Country}&");
                }

                if (requestParams.Language != null)
                {
                    actionUrlBuilder.Append($"language={requestParams.Language}&");
                }

                if (requestParams.Category != null)
                {
                    actionUrlBuilder.Append($"category={requestParams.Category}&");
                }

                if (requestParams.Sources != null && requestParams.Sources.Count > 0)
                {
                    actionUrlBuilder.Append($"sources={string.Join(",", requestParams.Sources)}&");
                }

                if (requestParams.Page > 0)
                {
                    actionUrlBuilder.Append($"page={requestParams.Page}&");
                }

                if (requestParams.PageSize > 0)
                {
                    actionUrlBuilder.Append($"pageSize={requestParams.PageSize}&");
                }
            }

            actionUrlBuilder.Append($"apiKey={ServiceConfig.NewsServiceApiKey}");
            var actionUrl = actionUrlBuilder.ToString();
            return actionUrl;
        }
    }
}