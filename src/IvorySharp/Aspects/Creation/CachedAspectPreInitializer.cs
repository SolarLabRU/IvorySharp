using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Comparers;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент для предварительной подготовки аспектов с поддержкой кеша.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    internal class CachedAspectPreInitializer<TAspect> : IAspectPreInitializer<TAspect> 
        where TAspect : OrderableMethodAspect
    {
        private readonly IComponentProvider<IAspectPreInitializer<TAspect>> _preInitializerProvider;
        private readonly ConcurrentDictionary<CacheKey, TAspect[]> _cache;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedAspectPreInitializer{TAspect}"/>.
        /// </summary>
        public CachedAspectPreInitializer(
            IComponentProvider<IAspectPreInitializer<TAspect>> preInitializerProvider)
        {
            _preInitializerProvider = preInitializerProvider;
            _cache = new ConcurrentDictionary<CacheKey, TAspect[]>();
        }

        /// <inheritdoc />
        public TAspect[] PrepareAspects(IInvocationContext context)
        {
            var key = new CacheKey(
                context.DeclaringType, 
                context.TargetType, 
                context.Method,
                context.TargetMethod);
            
            return _cache.GetOrAdd(key, _ => _preInitializerProvider.Get().PrepareAspects(context));
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

            /// <inheritdoc />
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