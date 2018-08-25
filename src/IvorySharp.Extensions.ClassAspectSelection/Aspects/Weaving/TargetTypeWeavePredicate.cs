using System;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions.ClassAspectSelection.Extensions;

namespace IvorySharp.Extensions.ClassAspectSelection.Aspects.Weaving
{
    /// <summary>
    /// Определяет возможность применения аспектов на основе целевого типа.
    /// </summary>
    internal sealed class TargetTypeWeavePredicate : BaseWeavePredicate
    {
        private readonly IKeyValueCache<TypePair, bool> _typeWeaveableCache;
        private readonly IKeyValueCache<IInvocationContext, bool> _invocationWeaveableCache;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="TargetTypeWeavePredicate"/>.
        /// </summary>
        public TargetTypeWeavePredicate(
            IComponentHolder<IAspectSelector> selectorHolder,
            IKeyValueCacheFactory cacheFactory) 
            : base(selectorHolder)
        {
            _typeWeaveableCache = cacheFactory.Create<TypePair, bool>();
            _invocationWeaveableCache = cacheFactory.Create<IInvocationContext, bool>(
                InvocationContextComparer.Instance);
        }     
        
        /// <inheritdoc />
        public override bool IsWeaveable(Type declaringType, Type targetType)
        {
            var key = new TypePair(targetType, declaringType);

            return _typeWeaveableCache.GetOrAdd(key, typePair =>
            {
                // В любом случае объявленный тип должен быть интерфейсом
                // это ограничение прокси-генератора, да и C#-а в целом
                if (!typePair.DeclaringType.IsInterface)
                    return false;

                if (!typePair.TargetType.IsClass || typePair.TargetType.IsAbstract)
                    return false;

                if (IsWeavingSuppressed(typePair.TargetType))
                    return false;

                var aspectSelector = AspectSelectorHolder.Get();

                // На интерфейсе есть аспект
                if (aspectSelector.HasAnyAspect(typePair.TargetType))
                    return true;

                // На методах класса есть аспекты
                if (typePair.TargetType.GetMethods().Any(
                    m => !IsWeavingSuppressed(m) &&
                         aspectSelector.HasAnyAspect(m)))
                    return true;

                var baseTypes = typePair.TargetType.GetInterceptableBaseTypes()
                    .Where(t => !IsWeavingSuppressed(t)).ToArray();

                // На базовых типах есть аспекты
                if (baseTypes.Any(t => aspectSelector.HasAnyAspect(t)))
                    return true;

                // На методах базового типа есть аспекты
                return baseTypes.SelectMany(i => i.GetMethods())
                    .Any(m => !IsWeavingSuppressed(m) &&
                              aspectSelector.HasAnyAspect(m));
            });
        }

        /// <inheritdoc />
        public override bool IsWeaveable(IInvocation invocation)
        {
            return _invocationWeaveableCache.GetOrAdd(invocation, inv =>
            {
                if (!inv.DeclaringType.IsInterface ||
                    !inv.TargetType.IsClass ||
                    inv.TargetType.IsAbstract)
                {
                    return false;
                }

                if (IsWeavingSuppressed(invocation.TargetMethod))
                    return false;

                var aspectSelector = AspectSelectorHolder.Get();

                return aspectSelector.HasAnyAspect(inv.TargetMethod) ||
                       IsWeaveable(inv.DeclaringType, inv.TargetType);
            });
        }
        
        /// <summary>
        /// Пара типов.
        /// </summary>
        private struct TypePair : IEquatable<TypePair>
        {
            /// <summary>
            /// Целевой тип.
            /// </summary>
            public readonly Type TargetType;
            
            /// <summary>
            /// Объявленный тип.
            /// </summary>
            public readonly Type DeclaringType;

            /// <summary>
            /// Инициализирует экземпляр <see cref="TypePair"/>.
            /// </summary>
            public TypePair(Type targetType, Type declaringType)
            {
                TargetType = targetType;
                DeclaringType = declaringType;
            }

            /// <inheritdoc />
            public bool Equals(TypePair other)
            {
                return TargetType == other.TargetType && DeclaringType == other.DeclaringType;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is TypePair tp && Equals(tp);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return (TargetType.GetHashCode() * 397) ^ DeclaringType.GetHashCode();
                }
            }
        }
    }
}