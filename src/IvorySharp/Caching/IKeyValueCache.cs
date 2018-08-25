using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Интерфейс кеша ключ-значение.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IKeyValueCache<TKey, TValue>
    {
        /// <summary>
        /// Получает значение по ключу из кеша. Если значения не было
        /// то оно добавляется в кеш.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="generator">Генератор значений.</param>
        /// <returns>Значение.</returns>
        TValue GetOrAdd([NotNull] TKey key, [NotNull] Func<TKey, TValue> generator);
    }
}