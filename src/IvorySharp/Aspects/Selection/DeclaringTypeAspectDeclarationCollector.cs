using System.Collections.Generic;
using System.Linq;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Собирает аспекты с типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal sealed class DeclaringTypeAspectDeclarationCollector : IAspectDeclarationCollector
    {
        private readonly IComponentProvider<IAspectSelector> _aspectSelectorProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeAspectDeclarationCollector"/>.
        /// </summary>
        /// <param name="aspectSelectorProvider">Провайдера компонента выбора аспектов.</param>
        public DeclaringTypeAspectDeclarationCollector(IComponentProvider<IAspectSelector> aspectSelectorProvider)
        {
            _aspectSelectorProvider = aspectSelectorProvider;
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(IInvocationContext context) 
            where TAspect : MethodAspect
        {
            var selector = _aspectSelectorProvider.Get();
            
            var methodAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.Method, includeAbstract: false);
            var typeAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.DeclaringType, includeAbstract: false)
                .Concat(context.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => selector.SelectAspectDeclarations<TAspect>(i, includeAbstract: false)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}