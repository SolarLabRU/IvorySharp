using System;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Селектор зависимостей аспектов.
    /// </summary>
    [PublicAPI]
    public interface IAspectDependencySelector : IComponent
    {
        /// <summary>
        /// Получает массив зависимостей на аспекте.
        /// </summary>
        /// <param name="aspectType">Тип аспекта.</param>
        /// <returns>Массив зависимостей-свойств аспекта.</returns>
        [NotNull] AspectPropertyDependency[] SelectPropertyDependencies([NotNull] Type aspectType);

        /// <summary>
        /// Возвращает признак наличия зависимостей на аспекте.
        /// </summary>
        /// <param name="aspectType">Тип аспекта.</param>
        /// <returns>Признак наличия зависимостей на аспекте.</returns>
        bool HasDependencies([NotNull] Type aspectType);
    }
}