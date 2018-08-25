using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Фабрика хранлищ ключ-значение.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IKeyValueCacheFactory
    {
        /// <summary>
        /// Создает экземпляр хранилища ключ-значение.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <returns>Хранилище ключ-значение.</returns>
        [NotNull] IKeyValueCache<TKey, TValue> Create<TKey, TValue>();
        
        /// <summary>
        ///  Создает экземпляр хранилища ключ-значение.
        /// </summary>
        /// <param name="comparer">Компаратор.</param>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <returns>Хранилище ключ-значение.</returns>
        [NotNull] IKeyValueCache<TKey, TValue> Create<TKey, TValue>([NotNull] IEqualityComparer<TKey> comparer);
    }
}