using System;
using System.Diagnostics;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Extensions;
using IvorySharp.Linq;
using IvorySharp.Reflection;

namespace IvorySharp.Caching
{
    /// <summary>
    /// Кеш методов.
    /// </summary>
    internal sealed class MethodInfoCache : IMethodInfoCache
    {
        private readonly IKeyValueCache<MethodMapCacheKey, MethodInfo> _methodMapCacheKey;
        private readonly IKeyValueCache<MethodInfo, bool> _asyncMethodCache;
        
        /// <summary>
        /// Инициализированный экземпляр <see cref="MethodInfoCache"/>.
        /// </summary>
        public static readonly MethodInfoCache Instance = new MethodInfoCache(
            ConcurrentDictionaryCacheFactory.Default);
        
        private MethodInfoCache(IKeyValueCacheFactory cacheFactory)
        {
            cacheFactory.Create<MethodInfo, MethodCall>(MethodEqualityComparer.Instance);
            _methodMapCacheKey = cacheFactory.Create<MethodMapCacheKey, MethodInfo>();
            _asyncMethodCache = cacheFactory.Create<MethodInfo, bool>(MethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public MethodInfo GetMethodMap(Type targetType, MethodInfo interfaceMethod)
        {
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(interfaceMethod != null, "interfaceMethod != null");
            
            var key = new MethodMapCacheKey(targetType, interfaceMethod);
            
            if (_methodMapCacheKey.TryGetValue(key, out var map))
                return map;

            map = ReflectedMethod.GetMethodMap(targetType, interfaceMethod);
                
            if (_methodMapCacheKey.TryAdd(key, map))
                return map;
                
            return _methodMapCacheKey.TryGetValue(key, out var cmap) ? cmap : map;
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
        private readonly struct MethodMapCacheKey
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