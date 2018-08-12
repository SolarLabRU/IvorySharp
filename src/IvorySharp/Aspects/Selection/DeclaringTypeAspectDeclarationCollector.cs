using System.Collections.Generic;
using System.Linq;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Собирает аспекты с типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal class DeclaringTypeAspectDeclarationCollector : IAspectDeclarationCollector
    {
        private readonly IAspectSelector _selector;

        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeAspectDeclarationCollector"/>.
        /// </summary>
        /// <param name="selector">Компонент выбора аспектов.</param>

        public DeclaringTypeAspectDeclarationCollector(IAspectSelector selector)
        {
            _selector = selector;
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(IInvocationContext context) 
            where TAspect : MethodAspect
        {
            var methodAspectDeclarations = _selector.SelectAspectDeclarations<TAspect>(context.Method, includeAbstract: false);
            var typeAspectDeclarations = _selector.SelectAspectDeclarations<TAspect>(context.DeclaringType, includeAbstract: false)
                .Concat(context.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => _selector.SelectAspectDeclarations<TAspect>(i, includeAbstract: false)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null && 
                            m.MulticastTarget != MethodAspectMulticastTarget.Undefined);

            return aspectDeclarations;
        }
    }
}