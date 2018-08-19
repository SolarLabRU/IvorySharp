using IvorySharp.Aspects.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    public interface IAspectDependencyInjector : IComponent
    {
        /// <summary>
        /// Выполняет внедрение зависимостей в аспект.
        /// </summary>
        /// <param name="aspect">Модель аспекта.</param>
        void InjectPropertyDependencies(MethodAspect aspect);
    }
}