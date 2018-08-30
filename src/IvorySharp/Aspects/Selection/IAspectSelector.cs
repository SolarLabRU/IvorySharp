using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент выбора аспектов с элементов.
    /// </summary>
    [PublicAPI]
    public interface IAspectSelector : IComponent
    {
        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        [NotNull] IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>([NotNull] Type type)
            where TAspect : MethodAspect;

        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        [NotNull] IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>([NotNull] MethodInfo method)
            where TAspect : MethodAspect;

        /// <summary>
        /// Выполняет проверку наличия аспекта на типе.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect([NotNull] Type type);

        /// <summary>
        /// Выполняет проверку наличия аспекта на методе.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect([NotNull] MethodInfo method);
    }
}