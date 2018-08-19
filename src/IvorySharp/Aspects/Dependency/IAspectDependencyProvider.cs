using System;
using System.Collections.Generic;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Интерфейс провайдера зависимостей аспекта.
    /// </summary>
    internal interface IAspectDependencyProvider : IComponent
    {
        /// <summary>
        /// Возвращает перечень свойств-зависимостей аспекта.
        /// </summary>
        /// <param name="aspectType">Тип аспекта.</param>
        /// <returns>Перечень зависимостей.</returns>
        [NotNull] IEnumerable<AspectPropertyDependency> GetPropertyDependencies([NotNull] Type aspectType);
    }
}