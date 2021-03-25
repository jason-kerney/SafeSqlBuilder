using System.Collections.Generic;

namespace SafeSqlBuilder.Helpers
{
    public static class TypeHelpers
    {
        public static bool IsNull<T>(this T thing) where T : class
        {
            return thing == null;
        }

        public static bool IsNotNull<T>(this T thing) where T : class
        {
            return !thing.IsNull();
        }

        public static bool IsNull<T>(this T? thing) where T : struct
        {
            return thing == null;
        }

        public static bool IsNotNull<T>(this T? thing) where T : struct
        {
            return !thing.IsNull();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> things)
        {
            // ReSharper disable PossibleMultipleEnumeration
            return things.IsNull() || things.Empty();
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}