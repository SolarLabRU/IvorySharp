using System;
using System.ComponentModel;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Селектор зависимостей аспектов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectDependencySelector : IComponent
    {
        /// <summary>
        /// Получает массив зависимостей на аспекте.
        /// </summary>
        /// <param name="aspectType">Тип аспекта.</param>
        /// <returns>Массив зависимостей-свойств аспекта.</returns>
        AspectPropertyDependency[] SelectPropertyDependencies(Type aspectType);
    }
}