using System;
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
    internal sealed class MethodInfoCache : IMethodInfoCache
    {
        private readonly IKeyValueCache<MethodInfo, MethodLambda> _invokerCache;
        private readonly IKeyValueCache<MethodMapCacheKey, MethodInfo> _methodMapCacheKey;
        private readonly IKeyValueCache<MethodInfo, bool> _asyncMethodCache;
        
        /// <summary>
        /// Инициализированный экземпляр <see cref="MethodInfoCache"/>.
        /// </summary>
        public static readonly MethodInfoCache Instance = new MethodInfoCache(
            ConcurrentDictionaryCacheFactory.Default);
        
        private MethodInfoCache(IKeyValueCacheFactory cacheFactory)
        {
            _invokerCache = cacheFactory.Create<MethodInfo, MethodLambda>(MethodEqualityComparer.Instance);
            _methodMapCacheKey = cacheFactory.Create<MethodMapCacheKey, MethodInfo>();
            _asyncMethodCache = cacheFactory.Create<MethodInfo, bool>(MethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public MethodLambda GetInvoker(MethodInfo method)
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
        public bool IsAsync(MethodInfo method)
        {
            Debug.Assert(method != null, "method != null");

            return _asyncMethodCache.GetOrAdd(method, m => m.IsAsync());
        }
        
        /// <summary>
        /// Ключ для хранения маппинга методов.
        /// </summary>
        private struct MethodMapCacheKey
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