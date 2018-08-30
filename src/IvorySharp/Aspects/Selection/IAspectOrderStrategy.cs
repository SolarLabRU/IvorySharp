using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Стратегия упорядочивания аспектов.
    /// </summary>
    [PublicAPI]
    public interface IAspectOrderStrategy : IComponent
    {
        /// <summary>
        /// Выполняет упорядочивание аспектов.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="aspects">Перечень аспектов для упорядочивания.</param>
        /// <returns>Упорядоченный набор аспектов.</returns>
        [NotNull] IOrderedEnumerable<TAspect> Order<TAspect>([NotNull] IEnumerable<TAspect> aspects) 
            where TAspect : OrderableMethodAspect;
    }
}