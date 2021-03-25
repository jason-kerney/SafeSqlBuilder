using System;
using System.Collections.Generic;
using System.Linq;

namespace SafeSqlBuilder.Helpers
{
    public static class EnumeratorHelper
    {
        public static bool Empty<T>(this IEnumerable<T> items)
        {
            return !items.Any();
        }

        public static bool Empty<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            return !items.Any(predicate);
        }
    }
}