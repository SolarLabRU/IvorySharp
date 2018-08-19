using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат возможности применения аспекта с кешем.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CachedWeavePredicate : IAspectWeavePredicate
    {
        private readonly ConcurrentDictionary<CacheKey, bool> _cache;
        private readonly IAspectWeavePredicate _predicate;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedWeavePredicate"/>.
        /// </summary>
        /// <param name="predicate">Исходный предикат.</param>
        public CachedWeavePredicate(IAspectWeavePredicate predicate)
        {
            _predicate = predicate;
            _cache = new ConcurrentDictionary<CacheKey, bool>();
        }
        
        /// <inheritdoc />
        public bool IsWeaveable(Type declaringType, Type targetType)
        {
            return _cache.GetOrAdd(new CacheKey(declaringType, targetType, method: null, targetMethod: null), 
                key => _predicate.IsWeaveable(key.DeclaringType, key.TargetType));
        }

        /// <inheritdoc />
        public bool IsWeaveable(IInvocation invocation)
        {
            var key = new CacheKey(
                invocation.DeclaringType, 
                invocation.TargetType, 
                invocation.Method, 
                invocation.TargetMethod);
            
            return _cache.GetOrAdd(key, _ => _predicate.IsWeaveable(invocation));
        }
        
        /// <summary>
        /// Ключ кеша.
        /// </summary>
        private class CacheKey
        {
            public readonly Type DeclaringType;
            public readonly Type TargetType;
            public readonly MethodInfo Method;
            public readonly MethodInfo TargetMethod;

            public CacheKey(Type declaringType, Type targetType, MethodInfo method, MethodInfo targetMethod)
            {
                DeclaringType = declaringType;
                TargetType = targetType;
                Method = method;
                TargetMethod = targetMethod;
            }

            public override bool Equals(object obj)
            {
                var key = obj as CacheKey;
                if (key == null)
                    return false;

                if (DeclaringType != key.DeclaringType)
                    return false;

                return TargetType == key.TargetType && 
                       MethodEqualityComparer.Instance.Equals(Method, key.Method) &&
                       MethodEqualityComparer.Instance.Equals(TargetMethod, key.TargetMethod);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                var hash = 0;
                if (DeclaringType != null)
                    hash ^= DeclaringType.GetHashCode();

                if (TargetType != null)
                    hash ^= TargetType.GetHashCode();

                if (Method != null)
                    hash ^= Method.GetHashCode();

                if (TargetMethod != null)
                    hash ^= TargetMethod.GetHashCode();
                
                return hash;
            }
        }
    }
}