using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Стратегия упорядочивания аспектов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
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