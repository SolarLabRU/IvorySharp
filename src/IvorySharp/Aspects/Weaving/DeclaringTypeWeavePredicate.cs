using System;
using System.Linq;
using IvorySharp.Aspects.Components;
using IvorySharp.Aspects.Selection;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат определяющий возможность применения аспектов на основе типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal sealed class DeclaringTypeWeavePredicate : BaseWeavePredicate
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeWeavePredicate"/>.
        /// </summary>
        public DeclaringTypeWeavePredicate(IComponentProvider<IAspectSelector> selector)
            : base(selector)
        {
        }

        /// <inheritdoc />
        public override bool IsWeaveable(Type declaringType, Type targetType)
        {
            if (!declaringType.IsInterface)
                return false;

            if (IsWeavingSuppressed(declaringType))
                return false;

            var selector = AspectSelectorProvider.Get();
            
            // На интерфейсе есть аспект
            if (selector.HasAnyAspect(declaringType, includeAbstract: false))
                return true;

            // На методах интерфейса есть аспекты
            if (declaringType.GetMethods().Any(m => !IsWeavingSuppressed(m) && selector.HasAnyAspect(m, includeAbstract: false)))
                return true;

            var interaces = declaringType.GetInterfaces().Where(i => !IsWeavingSuppressed(i)).ToArray();

            // На базовом интерфейсе есть аспект
            if (interaces.Any(i => selector.HasAnyAspect(i, includeAbstract: false)))
                return true;

            // На методах базового интерфейса есть аспекты
            return interaces.SelectMany(i => i.GetMethods())
                .Any(m => !IsWeavingSuppressed(m) && 
                           selector.HasAnyAspect(m, includeAbstract: false));
        }

        /// <inheritdoc />
        public override bool IsWeaveable(IInvocation invocation)
        {
            if (!invocation.DeclaringType.IsInterface)
                return false;

            if (IsWeavingSuppressed(invocation.Method))
                return false;

            var selector = AspectSelectorProvider.Get();
            
            return selector.HasAnyAspect(invocation.Method, includeAbstract: false) || 
                   IsWeaveable(invocation.DeclaringType, invocation.TargetType);
        }
    }
}