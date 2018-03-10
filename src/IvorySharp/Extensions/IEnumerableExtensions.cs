using System.Collections.Generic;
using System.Linq;

namespace IvorySharp.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNotEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source != null && source.Count > 0;
        }
        
        internal static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        internal static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }
    }
}