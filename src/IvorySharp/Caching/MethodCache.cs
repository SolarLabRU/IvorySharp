using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Reflection;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Кеш методов для быстрого вызова <see cref="MethodInfo"/>.
    /// </summary>
    internal class MethodCache
    {
        private readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> _cache;
        
        /// <summary>
        /// Инициализированный экземпляр <see cref="MethodCache"/>.
        /// </summary>
        public static readonly MethodCache Instance = new MethodCache();
        
        private MethodCache()
        {
            _cache = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>(MethodEqualityComparer.Instance);
        }
        
        /// <summary>
        /// Получает либо добавляет делегат вызова метода в кеш.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Делегат для быстрого вызова метода.</returns>
        public Func<object, object[], object> GetOrAdd(MethodInfo method)
        {
            return _cache.GetOrAdd(method, Expressions.CreateLambda);
        }
    }
}