using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Кеш на основе <see cref="ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConcurrentDictionaryCache<TKey, TValue> : IKeyValueCache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _concurrentDictionary;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ConcurrentDictionaryCache{TKey,TValue}"/>.
        /// </summary>
        public ConcurrentDictionaryCache()
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, TValue>();
        }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="ConcurrentDictionaryCache{TKey,TValue}"/>.
        /// </summary>
        /// <param name="comparer">Компаратор.</param>
        public ConcurrentDictionaryCache(IEqualityComparer<TKey> comparer)
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
        }

        /// <inheritdoc />
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> generator)
        {
            return _concurrentDictionary.GetOrAdd(key, generator);
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _concurrentDictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public bool TryAdd(TKey key, TValue value)
        {
            return _concurrentDictionary.TryAdd(key, value);
        }
    }
}