using System;
using System.Collections.Generic;
using System.Linq;

namespace News.Helpers
{
    public static class EnumerableHelper
    {
        public static bool IsNullOrEmpty<TItem>(this IEnumerable<TItem> source)
        {
            if (source == null)
                return false;

            if (source is ICollection<TItem> collection)
                return collection.Count == 0;

            return !source.Any();
        }
    }
}
