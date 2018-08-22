using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions.ClassAspectSelection.Extensions;

namespace IvorySharp.Extensions.ClassAspectSelection.Aspects.Selection
{
    /// <summary>
    /// Собирает объявления аспекты с целевого типа (класса) и методов класса.
    /// </summary>
    internal sealed class TargetTypeAspectDeclarationCollector : IAspectDeclarationCollector
    {
        private readonly IComponentHolder<IAspectSelector> _selectorHolder;

        /// <summary>
        /// Инициализирует экземпляр <see cref="TargetTypeAspectDeclarationCollector"/>.
        /// </summary>
        public TargetTypeAspectDeclarationCollector(IComponentHolder<IAspectSelector> selectorHolder)
        {
            _selectorHolder = selectorHolder;
        }
        
        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(
            IInvocationContext context) where TAspect : MethodAspect
        {
            var selector = _selectorHolder.Get();
            
            var methodAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.TargetMethod);
            
            var typeAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.TargetType)
                .Concat(context.TargetType.GetInterceptableBaseTypes()
                    .SelectMany(t => selector.SelectAspectDeclarations<TAspect>(t)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}