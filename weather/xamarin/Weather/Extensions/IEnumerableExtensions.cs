using System;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Extensions
{
    public static class IEnumerableExtensions
    {
        private static Random random = new Random(Environment.TickCount);

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null || !enumerable.Any())
            {
                throw new ArgumentOutOfRangeException();
            }

            return enumerable.ElementAt(random.Next(minValue: 0, maxValue: enumerable.Count()));
        }
    }
}
