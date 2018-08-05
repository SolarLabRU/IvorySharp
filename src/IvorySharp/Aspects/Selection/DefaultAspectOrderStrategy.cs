using System.Collections.Generic;
using System.Linq;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Стратегия сортировки аспектов по умолчанию.
    /// Сначала аспекты упорядочиваются по возрастанию <see cref="OrderableMethodAspect.Order"/>, затем по
    /// <see cref="MethodAspect.JoinPointType"/>.
    /// </summary>
    internal class DefaultAspectOrderStrategy : IAspectOrderStrategy
    {
        /// <inheritdoc />
        public IOrderedEnumerable<TAspect> Order<TAspect>(IEnumerable<TAspect> aspects) where TAspect : OrderableMethodAspect
        {
            return aspects.OrderBy(a => a.Order).ThenBy(a => a.JoinPointType);
        }
    }
}