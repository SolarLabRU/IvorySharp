using System.Linq;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Components.Selection
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
        /// <param name="selector">Стратегия выбора аспектов.</param>

        public DeclaringTypeAspectDeclarationCollector(IAspectSelector selector)
        {
            _selector = selector;
        }

        /// <inheritdoc />
        public MethodAspectDeclaration<TAspect>[] CollectAspectDeclarations<TAspect>(InvocationContext context) where TAspect : MethodAspect
        {
            var methodAspectDeclarations = _selector.SelectAspectDeclarations<TAspect>(context.Method, includeAbstract: false);
            var typeAspectDeclarations = _selector.SelectAspectDeclarations<TAspect>(context.DeclaringType, includeAbstract: false)
                .Concat(context.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => _selector.SelectAspectDeclarations<TAspect>(i, includeAbstract: false)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null)
                .Distinct(MethodAspectDeclaration<TAspect>.ByAspectTypeEqualityComparer.Instance)
                .ToArray();

            return aspectDeclarations;
        }
    }
}