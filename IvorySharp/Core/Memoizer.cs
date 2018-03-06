using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IvorySharp.Core
{
    /// <summary>
    /// Реализация шаблона "Memoize".
    /// </summary>
    internal static class Memoizer
    {
        /// <summary>
        /// Сохраняет первичный результат выполнения в кеш. При последующих обращения возвращает сохраненный результат.
        /// </summary>
        /// <param name="handler">Делегар, результат которого необходимо сохранить.</param>
        /// <typeparam name="TIn">Тип входного параметра.</typeparam>
        /// <typeparam name="TOut">Тип выходного параметра.</typeparam>
        /// <returns>Новый делегат, который возвращает сохраненный результат первичного вычисления при вызове.</returns>
        public static Func<TIn, TOut> Memoize<TIn, TOut>(Func<TIn, TOut> handler)
        {
            var cache = new ConcurrentDictionary<TIn, TOut>();
            return arg => cache.GetOrAdd(arg, handler);
        }
        
        /// <summary>
        /// Сохраняет первичный результат выполнения в кеш. При последующих обращения возвращает сохраненный результат.
        /// </summary>
        /// <param name="handler">Делегар, результат которого необходимо сохранить.</param>
        /// <param name="comparer">Компаратор для кеша.</param>
        /// <typeparam name="TIn">Тип входного параметра.</typeparam>
        /// <typeparam name="TOut">Тип выходного параметра.</typeparam>
        /// <returns>Новый делегат, который возвращает сохраненный результат первичного вычисления при вызове.</returns>
        public static Func<TIn, TOut> Memoize<TIn, TOut>(Func<TIn, TOut> handler, IEqualityComparer<TIn> comparer)
        {
            var cache = new ConcurrentDictionary<TIn, TOut>(comparer);
            return arg => cache.GetOrAdd(arg, handler);
        }
    }
}