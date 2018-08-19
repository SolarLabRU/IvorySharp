using System.Collections.Generic;
using System.Diagnostics;

namespace IvorySharp.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeBefore<T>(this IEnumerable<T> source, T target) 
            where T : class
        {
            Debug.Assert(target != null, "target != null");

            foreach (var item in source)
            {
                if (ReferenceEquals(item, target))
                    yield break;

                yield return item;
            }
        }
    }
}