using System;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат определяющий возможность применения аспектов на основе типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal sealed class DeclaringTypeWeavePredicate : BaseWeavePredicate
    {
        private IKeyValueCache<Type, bool> _typeWeaveableCache;
        private IKeyValueCache<IInvocationContext, bool> _invocationWeaveableCache;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeWeavePredicate"/>.
        /// </summary>
        public DeclaringTypeWeavePredicate(
            IComponentHolder<IAspectSelector> selector,
            IKeyValueCacheFactory cacheFactory)
            : base(selector)
        {
            _typeWeaveableCache = cacheFactory.Create<Type, bool>();
            _invocationWeaveableCache = cacheFactory.Create<IInvocationContext, bool>(
                InvocationContextComparer.Instance);
        }

        /// <inheritdoc />
        public override bool IsWeaveable(Type declaringType, Type targetType)
        {
            return _typeWeaveableCache.GetOrAdd(declaringType, dt =>
            {
                if (!dt.IsInterface)
                    return false;

                if (IsWeavingSuppressed(dt))
                    return false;

                var selector = AspectSelectorHolder.Get();

                // На интерфейсе есть аспект
                if (selector.HasAnyAspect(dt))
                    return true;

                // На методах интерфейса есть аспекты
                if (dt.GetMethods().Any(m => !IsWeavingSuppressed(m) && selector.HasAnyAspect(m)))
                    return true;

                var interaces = dt.GetInterfaces().Where(i => !IsWeavingSuppressed(i)).ToArray();

                // На базовом интерфейсе есть аспект
                if (interaces.Any(i => selector.HasAnyAspect(i)))
                    return true;

                // На методах базового интерфейса есть аспекты
                return interaces.SelectMany(i => i.GetMethods())
                    .Any(m => !IsWeavingSuppressed(m) &&
                              selector.HasAnyAspect(m));
            });
        }

        /// <inheritdoc />
        public override bool IsWeaveable(IInvocation invocation)
        {
            return _invocationWeaveableCache.GetOrAdd(invocation, iv =>
            {
                if (!iv.DeclaringType.IsInterface)
                    return false;

                if (IsWeavingSuppressed(iv.Method))
                    return false;

                var selector = AspectSelectorHolder.Get();

                return selector.HasAnyAspect(iv.Method) ||
                       IsWeaveable(iv.DeclaringType, iv.TargetType);
            });
        }
    }
}