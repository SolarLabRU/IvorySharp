using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Components;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Стратегия упорядочивания аспектов.
    /// </summary>
    public interface IAspectOrderStrategy : IComponent
    {
        /// <summary>
        /// Выполняет упорядочивание аспектов.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="aspects">Перечень аспектов для упорядочивания.</param>
        /// <returns>Упорядоченный набор аспектов.</returns>
        IOrderedEnumerable<TAspect> Order<TAspect>(IEnumerable<TAspect> aspects) 
            where TAspect : OrderableMethodAspect;
    }
}