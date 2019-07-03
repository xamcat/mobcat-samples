using System.Collections.Generic;

namespace News.Models
{
    public class FetchArticlesResponse : PageableResult
    {
        public List<Article> Articles { get; set; } = new List<Article>();

        public FetchArticlesResponse(int pageNumber = 0, int pageSize = 0)
            : base(pageNumber, pageSize)
        {

        }
    }
}
