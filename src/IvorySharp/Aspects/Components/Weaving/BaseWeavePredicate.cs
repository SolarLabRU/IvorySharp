using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Components.Weaving
{
    /// <summary>
    /// Базовый класс предиката возможности применения аспектов.
    /// </summary>
    internal abstract class BaseWeavePredicate : IMethodAspectWeavePredicate
    {
        /// <summary>
        /// Стратегия выбора аспектов.
        /// </summary>
        protected IMethodAspectSelectionStrategy AspectSelectionStrategy { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="BaseWeavePredicate"/>.
        /// </summary>
        /// <param name="selectionStrategy">Страгетия выбора аспектов.</param>
        protected BaseWeavePredicate(IMethodAspectSelectionStrategy selectionStrategy)
        {
            AspectSelectionStrategy = selectionStrategy;
        }

        /// <inheritdoc />
        public abstract bool IsWeaveable(Type declaringType, Type targetType);

        /// <inheritdoc />
        public abstract bool IsWeaveable(MethodInfo method, Type declaringType, Type targetType);

        /// <summary>
        /// Возвращает признак того, что применение аспектов запрещено.
        /// </summary>
        protected bool IsWeavingSuppressed(Type type)
        {
            return !type.IsInterceptable() || type.GetCustomAttributes<SuppressAspectsWeavingAttribute>(inherit: false).Any();
        }

        /// <summary>
        /// Возвращает признак того, что применение аспектов запрещено.
        /// </summary>
        protected bool IsWeavingSuppressed(MethodInfo method)
        {
            return !method.IsInterceptable() || method.GetCustomAttributes<SuppressAspectsWeavingAttribute>(inherit: false).Any();
        }
    }
}