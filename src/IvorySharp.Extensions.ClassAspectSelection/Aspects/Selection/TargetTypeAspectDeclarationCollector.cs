using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components;
using IvorySharp.Aspects.Selection;
using IvorySharp.Core;
using IvorySharp.Extensions.ClassAspectSelection.Extensions;

namespace IvorySharp.Extensions.ClassAspectSelection.Aspects.Selection
{
    /// <summary>
    /// Собирает объявления аспекты с целевого типа (класса) и методов класса.
    /// </summary>
    internal sealed class TargetTypeAspectDeclarationCollector : IAspectDeclarationCollector
    {
        private readonly IComponentProvider<IAspectSelector> _selectorProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="TargetTypeAspectDeclarationCollector"/>.
        /// </summary>
        public TargetTypeAspectDeclarationCollector(IComponentProvider<IAspectSelector> selectorProvider)
        {
            _selectorProvider = selectorProvider;
        }
        
        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(
            IInvocationContext context) where TAspect : MethodAspect
        {
            var selector = _selectorProvider.Get();
            
            var methodAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(
                context.TargetMethod, includeAbstract: false);
            
            var typeAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(
                    context.TargetType, includeAbstract: false)
                .Concat(context.TargetType.GetInterceptableBaseTypes()
                    .SelectMany(t => selector.SelectAspectDeclarations<TAspect>(t, includeAbstract: false)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}