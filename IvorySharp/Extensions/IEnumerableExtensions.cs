using System.Collections.Generic;

namespace IvorySharp.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool IsNotEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source != null && source.Count > 0;
        }
    }
}