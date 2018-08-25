using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Интерфейс фабрики делегатов, берущих значение из кеша по входному параметру метода.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICacheDelegateFactory
    {
        /// <summary>
        /// Создает экземпляр делегата, получающего значение из кеша.
        /// </summary>
        /// <param name="generator">Генератор значений.</param>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <returns>ДЭкземпляр делегата.</returns>
        Func<TKey, TValue> CreateDelegate<TKey, TValue>(
            Func<TKey, TValue> generator);
        
        /// <summary>
        /// Создает экземпляр делегата, получающего значение из кеша.
        /// </summary>
        /// <param name="generator">Генератор значений.</param>
        /// <param name="comparer">Компаратор.</param>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <returns>ДЭкземпляр делегата.</returns>
        Func<TKey, TValue> CreateDelegate<TKey, TValue>(
            Func<TKey, TValue> generator, IEqualityComparer<TKey> comparer);
    }
}