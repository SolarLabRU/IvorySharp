using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Components.Selection;

namespace IvorySharp.Aspects.Components.Weaving
{
    /// <summary>
    /// Предикат определяющий возможность применения аспектов на основе типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal class DeclaringTypeWeavePredicate : BaseWeavePredicate
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeWeavePredicate"/>.
        /// </summary>
        /// <param name="selector">Компонент выбора аспектов.</param>
        public DeclaringTypeWeavePredicate(IAspectSelector selector)
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

            // На интерфейсе есть аспект
            if (AspectSelector.HasAnyAspect(declaringType, includeAbstract: false))
                return true;

            // На методах интерфейсва есть аспекты
            if (declaringType.GetMethods().Any(m => !IsWeavingSuppressed(m) && AspectSelector.HasAnyAspect(m, includeAbstract: false)))
                return true;

            var interaces = declaringType.GetInterfaces().Where(i => !IsWeavingSuppressed(i)).ToArray();

            // На базовом интерфейсе есть аспект
            if (interaces.Any(i => AspectSelector.HasAnyAspect(i, includeAbstract: false)))
                return true;

            // На методах базового интерфейса есть аспекты
            if (interaces.SelectMany(i => i.GetMethods()).Any(m => !IsWeavingSuppressed(m) && AspectSelector.HasAnyAspect(m, includeAbstract: false)))
                return true;

            return false;
        }

        /// <inheritdoc />
        public override bool IsWeaveable(MethodInfo method, Type declaringType, Type targetType)
        {
            if (!declaringType.IsInterface)
                return false;

            if (IsWeavingSuppressed(method))
                return false;

            if (AspectSelector.HasAnyAspect(method, includeAbstract: false))
                return true;

            return IsWeaveable(declaringType, targetType);
        }
    }
}