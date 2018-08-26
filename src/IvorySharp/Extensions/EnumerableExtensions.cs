using System.Collections.Generic;
using System.Diagnostics;

namespace IvorySharp.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool ContainsReference<T>(this IEnumerable<T> source, T target)
        {
            Debug.Assert(target != null, "target != null");

            foreach (var item in source)
            {
                if (ReferenceEquals(item, target))
                    return true;
            }
            
            return false;
        }
        
        public static IEnumerable<T> TakeBeforeExclusive<T>(this IEnumerable<T> source, T target) 
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
        
        public static IEnumerable<T> TakeBeforeInclusive<T>(this IEnumerable<T> source, T target) 
            where T : class
        {
            Debug.Assert(target != null, "target != null");

            foreach (var item in source)
            {
                if (ReferenceEquals(item, target))
                {
                    yield return item;
                    yield break;
                }

                yield return item;
            }
        }
    }
}