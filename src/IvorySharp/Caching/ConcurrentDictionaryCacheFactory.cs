using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Фабрика <see cref="ConcurrentDictionaryCache{TKey,TValue}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConcurrentDictionaryCacheFactory : IKeyValueCacheFactory
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="ConcurrentDictionaryCacheFactory"/>.
        /// </summary>
        public static readonly ConcurrentDictionaryCacheFactory Default 
            = new ConcurrentDictionaryCacheFactory();
        
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConcurrentDictionaryCacheFactory() { }
        
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IKeyValueCache<TKey, TValue> Create<TKey, TValue>()
        {
            return new ConcurrentDictionaryCache<TKey, TValue>();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IKeyValueCache<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return new ConcurrentDictionaryCache<TKey, TValue>(comparer);
        }
    }
}