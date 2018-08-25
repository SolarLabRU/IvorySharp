using System.Collections.Generic;
using IvorySharp.Caching;

namespace IvorySharp.Tests.Assets.Cache
{
    public class NullKeyValueCacheFactory : IKeyValueCacheFactory
    {
        public IKeyValueCache<TKey, TValue> Create<TKey, TValue>()
        {
            return new NullKeyValueCache<TKey, TValue>();
        }

        public IKeyValueCache<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return new NullKeyValueCache<TKey, TValue>();
        }
    }
}