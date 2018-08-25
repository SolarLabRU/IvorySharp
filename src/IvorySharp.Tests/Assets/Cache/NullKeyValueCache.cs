using System;
using IvorySharp.Caching;

namespace IvorySharp.Tests.Assets.Cache
{
    public class NullKeyValueCache<TKey, TValue> : IKeyValueCache<TKey, TValue>
    {
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> generator)
        {
            return generator(key);
        }
    }
}