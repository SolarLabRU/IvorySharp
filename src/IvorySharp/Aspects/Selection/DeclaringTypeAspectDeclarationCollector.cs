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
        private readonly IComponentHolder<IAspectSelector> _aspectSelectorHolder;

        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeAspectDeclarationCollector"/>.
        /// </summary>
        /// <param name="aspectSelectorHolder">Провайдера компонента выбора аспектов.</param>
        public DeclaringTypeAspectDeclarationCollector(IComponentHolder<IAspectSelector> aspectSelectorHolder)
        {
            _aspectSelectorHolder = aspectSelectorHolder;
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(IInvocationContext context) 
            where TAspect : MethodAspect
        {
            var selector = _aspectSelectorHolder.Get();
            
            var methodAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.Method);
            var typeAspectDeclarations = selector.SelectAspectDeclarations<TAspect>(context.DeclaringType)
                .Concat(context.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => selector.SelectAspectDeclarations<TAspect>(i)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}