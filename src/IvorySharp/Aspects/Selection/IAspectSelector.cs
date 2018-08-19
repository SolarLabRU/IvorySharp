using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Components;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент выбора аспектов с элементов.
    /// </summary>
    public interface IAspectSelector : IComponent
    {
        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="includeAbstract">Признак необходимости включить в выборку абстрактные аспекты.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(Type type, bool includeAbstract)
            where TAspect : MethodAspect;

        /// <summary>
        /// Возвращает коллекцию аспектов определенного типа.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <param name="includeAbstract">Признак необходимости включить в выборку абстрактные аспекты.</param>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <returns>Перечень аспектов.</returns>
        IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(MethodInfo method, bool includeAbstract)
            where TAspect : MethodAspect;

        /// <summary>
        /// Выполняет проверку наличия аспекта на типе.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="includeAbstract">Признак необходимости включить в выборку абстрактные аспекты.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect(Type type, bool includeAbstract);

        /// <summary>
        /// Выполняет проверку наличия аспекта на методе.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <param name="includeAbstract">Признак необходимости включить в выборку абстрактные аспекты.</param>
        /// <returns>Признак наличия аспектов.</returns>
        bool HasAnyAspect(MethodInfo method, bool includeAbstract);
    }
}