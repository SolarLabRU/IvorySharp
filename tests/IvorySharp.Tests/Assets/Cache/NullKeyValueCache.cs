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

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return false;
        }
    }
}