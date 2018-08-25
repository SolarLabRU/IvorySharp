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
        private IAspectSelector _aspectSelector;

        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeAspectDeclarationCollector"/>.
        /// </summary>
        /// <param name="aspectSelectorHolder">Провайдера компонента выбора аспектов.</param>
        public DeclaringTypeAspectDeclarationCollector(IComponentHolder<IAspectSelector> aspectSelectorHolder)
        {
            _aspectSelectorHolder = aspectSelectorHolder;
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(
            IInvocationSignature signature) where TAspect : MethodAspect
        {
            if (_aspectSelector == null)
                _aspectSelector = _aspectSelectorHolder.Get();
            
            var methodAspectDeclarations = _aspectSelector.SelectAspectDeclarations<TAspect>(signature.Method);
            var typeAspectDeclarations = _aspectSelector.SelectAspectDeclarations<TAspect>(signature.DeclaringType)
                .Concat(signature.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => _aspectSelector.SelectAspectDeclarations<TAspect>(i)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}