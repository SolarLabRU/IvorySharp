using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Селектор аспектов.
    /// </summary>
    public interface IMethodAspectSelector
    {
        /// <summary>
        /// Получает коллекцию связанных с перехватываемым методом аспектов. 
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Коллекция аспектов.</returns>
        IReadOnlyCollection<IMethodBoundaryAspect> GetMethodBoundaryAspects(IInvocation invocation);
    }
}