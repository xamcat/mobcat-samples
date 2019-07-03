using System.Collections.Generic;
using News.ViewModels;

namespace News.Models
{
    public class FetchArticlesResult : PageableResult
    {
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();

        public FetchArticlesResult(int pageNumber = 0, int pageSize = 0)
          : base(pageNumber, pageSize)
        {

        }
    }
}
