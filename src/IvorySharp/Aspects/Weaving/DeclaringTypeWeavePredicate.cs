using System;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
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
        public DeclaringTypeWeavePredicate(
            IComponentHolder<IAspectSelector> selector)
            : base(selector)
        {
        }

        /// <inheritdoc />
        public override bool IsWeaveable(Type declaredType, Type targetType)
        {
            if (!declaredType.IsInterface)
                return false;

            if (IsWeavingSuppressed(declaredType))
                return false;

            var selector = AspectSelectorHolder.Get();

            // На интерфейсе есть аспект
            if (selector.HasAnyAspect(declaredType))
                return true;

            // На методах интерфейса есть аспекты
            if (declaredType.GetMethods().Any(m => !IsWeavingSuppressed(m) && selector.HasAnyAspect(m)))
                return true;

            var interaces = declaredType.GetInterfaces().Where(i => !IsWeavingSuppressed(i)).ToArray();

            // На базовом интерфейсе есть аспект
            if (interaces.Any(i => selector.HasAnyAspect(i)))
                return true;

            // На методах базового интерфейса есть аспекты
            return interaces.SelectMany(i => i.GetMethods())
                .Any(m => !IsWeavingSuppressed(m) &&
                          selector.HasAnyAspect(m));
        }

        /// <inheritdoc />
        public override bool IsWeaveable(IInvocationSignature signature)
        {
            if (!signature.DeclaringType.IsInterface)
                return false;

            if (IsWeavingSuppressed(signature.Method))
                return false;

            var selector = AspectSelectorHolder.Get();

            return selector.HasAnyAspect(signature.Method) ||
                   IsWeaveable(signature.DeclaringType, signature.TargetType);
        }
    }
}