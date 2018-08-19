using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Comparers;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат возможности применения аспекта с кешем.
    /// </summary>
    internal sealed class CachedWeavePredicate : IAspectWeavePredicate
    {
        private readonly ConcurrentDictionary<CacheKey, bool> _cache;
        private readonly IAspectWeavePredicate _predicate;

        public CachedWeavePredicate(IAspectWeavePredicate predicate)
        {
            _predicate = predicate;
            _cache = new ConcurrentDictionary<CacheKey, bool>();
        }
        
        /// <inheritdoc />
        public bool IsWeaveable(Type declaringType, Type targetType)
        {
            return _cache.GetOrAdd(new CacheKey(declaringType, targetType, null), 
                key => _predicate.IsWeaveable(key.DeclaringType, key.TargetType));
        }

        /// <inheritdoc />
        public bool IsWeaveable(MethodInfo method, Type declaringType, Type targetType)
        {
            return _cache.GetOrAdd(new CacheKey(declaringType, targetType, method),
                key => _predicate.IsWeaveable(key.Method, key.DeclaringType, key.TargetType));
        }
        
        /// <summary>
        /// Ключ кеша.
        /// </summary>
        private class CacheKey
        {
            public readonly Type DeclaringType;
            public readonly Type TargetType;
            public readonly MethodInfo Method;

            public CacheKey(Type declaringType, Type targetType, MethodInfo method)
            {
                DeclaringType = declaringType;
                TargetType = targetType;
                Method = method;
            }

            public override bool Equals(object obj)
            {
                var key = obj as CacheKey;
                if (key == null)
                    return false;

                if (DeclaringType != key.DeclaringType)
                    return false;

                return TargetType == key.TargetType && 
                       MethodEqualityComparer.Instance.Equals(Method, key.Method);
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

                return hash;
            }
        }
    }
}