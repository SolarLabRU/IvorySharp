using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент выбора аспектов с элементов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectSelector : IComponent
    {
        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(Type type)
            where TAspect : MethodAspect;

        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(MethodInfo method)
            where TAspect : MethodAspect;

        /// <summary>
        /// Выполняет проверку наличия аспекта на типе.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect(Type type);

        /// <summary>
        /// Выполняет проверку наличия аспекта на методе.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect(MethodInfo method);
    }
}