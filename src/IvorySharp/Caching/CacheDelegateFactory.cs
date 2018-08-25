using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Фабрика делегатов.
    /// </summary>
    /// <typeparam name="TCacheFactory">Тип хранилища.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CacheDelegateFactory<TCacheFactory>: ICacheDelegateFactory
        where TCacheFactory : IKeyValueCacheFactory, new()
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="CacheDelegateFactory{TCacheFactory}"/>.ß
        /// </summary>
        public static readonly CacheDelegateFactory<TCacheFactory> Instance 
            = new CacheDelegateFactory<TCacheFactory>();
        
        private readonly IKeyValueCacheFactory _keyValueCacheFactory;
        
        private CacheDelegateFactory()
        {
            _keyValueCacheFactory = new TCacheFactory();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]       
        public Func<TKey, TValue> CreateDelegate<TKey, TValue>(Func<TKey, TValue> generator)
        {
            var store = _keyValueCacheFactory.Create<TKey, TValue>();
            return k => store.GetOrAdd(k, generator);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Func<TKey, TValue> CreateDelegate<TKey, TValue>(
            Func<TKey, TValue> generator, IEqualityComparer<TKey> comparer)
        {
            var store = _keyValueCacheFactory.Create<TKey, TValue>(comparer);
            return k => store.GetOrAdd(k, generator);
        }
    }
}