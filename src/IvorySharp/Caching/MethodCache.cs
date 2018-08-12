using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Кеш методов.
    /// </summary>
    internal class MethodCache
    {
        private readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> _invokerCache;
        private readonly ConcurrentDictionary<MethodMapCacheKey, MethodInfo> _methodMapCacheKey;
        
        /// <summary>
        /// Инициализированный экземпляр <see cref="MethodCache"/>.
        /// </summary>
        public static readonly MethodCache Instance = new MethodCache();
        
        private MethodCache()
        {
            _invokerCache = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>(MethodEqualityComparer.Instance);
            _methodMapCacheKey = new ConcurrentDictionary<MethodMapCacheKey, MethodInfo>();
        }
        
        /// <summary>
        /// Получает либо добавляет делегат вызова метода в кеш.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Делегат для быстрого вызова метода.</returns>
        [NotNull] public Func<object, object[], object> GetMethodInvoker([NotNull] MethodInfo method)
        {
            return _invokerCache.GetOrAdd(method, Expressions.CreateLambda);
        }

        /// <summary>
        /// Возвращает метод в типе <paramref name="targetType"/>, соответствующий методу <paramref name="interfaceMethod"/>.
        /// </summary>
        /// <param name="targetType">Тип класса.</param>
        /// <param name="interfaceMethod">Метод в интерфейсе.</param>
        /// <returns>Метод, соответствующий <paramref name="interfaceMethod"/>.</returns>
        [CanBeNull] public MethodInfo GetMethodMap([NotNull] Type targetType, [NotNull] MethodInfo interfaceMethod)
        {
            var key = new MethodMapCacheKey(targetType, interfaceMethod);
            return _methodMapCacheKey.GetOrAdd(key, k => ReflectedMethod.GetMethodMap(targetType, interfaceMethod));
        }
        
        /// <summary>
        /// Ключ для хранения маппинга методов.
        /// </summary>
        internal struct MethodMapCacheKey
        {
            public readonly Type TargetType;
            public readonly MethodInfo InterfaceMethod;

            /// <summary>
            /// Инициализирует экземпляр <see cref="MethodMapCacheKey"/>.
            /// </summary>
            public MethodMapCacheKey(Type targetType, MethodInfo interfaceMethod)
            {
                TargetType = targetType;
                InterfaceMethod = interfaceMethod;
            }

            private bool Equals(MethodMapCacheKey other)
            {
                return TargetType == other.TargetType && 
                       Equals(InterfaceMethod, other.InterfaceMethod);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) 
                    return false;

                return obj.GetType() == GetType() 
                       && Equals((MethodMapCacheKey) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((TargetType != null ? TargetType.GetHashCode() : 0) * 397) ^ 
                           (InterfaceMethod != null ? InterfaceMethod.GetHashCode() : 0);
                }
            }
        }
    }
}