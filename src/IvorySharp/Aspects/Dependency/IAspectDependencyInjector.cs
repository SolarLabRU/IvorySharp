using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    [PublicAPI]
    public interface IAspectDependencyInjector : IComponent
    {
        /// <summary>
        /// Выполняет внедрение зависимостей в аспект.
        /// </summary>
        /// <param name="aspect">Модель аспекта.</param>
        void InjectPropertyDependencies([NotNull] MethodAspect aspect);
    }
}