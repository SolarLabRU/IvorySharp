using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Selection;

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
            return interaces.SelectMany(i => i.GetMethods())
                .Any(m => !IsWeavingSuppressed(m) && 
                          AspectSelector.HasAnyAspect(m, includeAbstract: false));
        }

        /// <inheritdoc />
        public override bool IsWeaveable(MethodInfo method, Type declaringType, Type targetType)
        {
            if (!declaringType.IsInterface)
                return false;

            if (IsWeavingSuppressed(method))
                return false;

            return AspectSelector.HasAnyAspect(method, includeAbstract: false) || 
                   IsWeaveable(declaringType, targetType);
        }
    }
}