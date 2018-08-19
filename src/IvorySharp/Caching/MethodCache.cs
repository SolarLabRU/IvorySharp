using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Кеш методов.
    /// </summary>
    internal sealed class MethodCache : IMethodCache
    {
        private readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> _invokerCache;
        private readonly ConcurrentDictionary<MethodMapCacheKey, MethodInfo> _methodMapCacheKey;
        private readonly ConcurrentDictionary<MethodInfo, bool> _asyncMethodCache;
        
        /// <summary>
        /// Инициализированный экземпляр <see cref="MethodCache"/>.
        /// </summary>
        public static readonly MethodCache Instance = new MethodCache();
        
        private MethodCache()
        {
            _invokerCache = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>(MethodEqualityComparer.Instance);
            _methodMapCacheKey = new ConcurrentDictionary<MethodMapCacheKey, MethodInfo>();
            _asyncMethodCache = new ConcurrentDictionary<MethodInfo, bool>(MethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public Func<object, object[], object> GetInvoker(MethodInfo method)
        {
            Debug.Assert(method != null, "method != null");
            
            return _invokerCache.GetOrAdd(method, Expressions.CreateLambda);
        }

        /// <inheritdoc />
        public MethodInfo GetMethodMap(Type targetType, MethodInfo interfaceMethod)
        {
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(interfaceMethod != null, "interfaceMethod != null");
            
            var key = new MethodMapCacheKey(targetType, interfaceMethod);
            return _methodMapCacheKey.GetOrAdd(key, k => ReflectedMethod.GetMethodMap(targetType, interfaceMethod));
        }

        /// <inheritdoc />
        public bool GetIsAsync(MethodInfo method)
        {
            Debug.Assert(method != null, "method != null");

            return _asyncMethodCache.GetOrAdd(method, m => m.IsAsync());
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