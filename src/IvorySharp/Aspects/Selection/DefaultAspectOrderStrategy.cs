using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Стратегия сортировки аспектов по умолчанию.
    /// Сначала аспекты упорядочиваются по возрастанию <see cref="OrderableMethodAspect.Order"/>, затем по
    /// <see cref="MethodAspect.MulticastTarget"/>.
    /// </summary>
    internal sealed class DefaultAspectOrderStrategy : IAspectOrderStrategy
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TAspect> Order<TAspect>(IEnumerable<TAspect> aspects)
            where TAspect : OrderableMethodAspect
        {
            return aspects
                .OrderBy(a => a.Order)
                .ThenBy(a => a.MulticastTarget);
        }
    }
}