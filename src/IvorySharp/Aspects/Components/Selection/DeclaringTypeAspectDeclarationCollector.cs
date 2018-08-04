using System.Linq;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Components.Selection
{
    /// <summary>
    /// Собирает аспекты с типа, в котором объявлен перехватываемый метод.
    /// </summary>
    internal class DeclaringTypeAspectDeclarationCollector : IMethodAspectDeclarationCollector
    {
        private readonly IMethodAspectSelectionStrategy _selectionStrategy;

        /// <summary>
        /// Инициализирует экземпляр <see cref="DeclaringTypeAspectDeclarationCollector"/>.
        /// </summary>
        /// <param name="selectionStrategy">Стратегия выбора аспектов.</param>

        public DeclaringTypeAspectDeclarationCollector(IMethodAspectSelectionStrategy selectionStrategy)
        {
            _selectionStrategy = selectionStrategy;
        }

        /// <inheritdoc />
        public MethodAspectDeclaration<TAspect>[] CollectAspectDeclarations<TAspect>(InvocationContext context) where TAspect : MethodAspect
        {
            var methodAspectDeclarations = _selectionStrategy.GetDeclarations<TAspect>(context.Method, includeAbstract: false);
            var typeAspectDeclarations = _selectionStrategy.GetDeclarations<TAspect>(context.DeclaringType, includeAbstract: false)
                .Concat(context.DeclaringType.GetInterceptableInterfaces()
                    .SelectMany(i => _selectionStrategy.GetDeclarations<TAspect>(i, includeAbstract: false)));

            var aspectDeclarations = typeAspectDeclarations
                .Concat(methodAspectDeclarations)
                .Where(m => m.MethodAspect != null)
                .Distinct(MethodAspectDeclaration<TAspect>.ByAspectTypeEqualityComparer.Instance)
                .ToArray();

            return aspectDeclarations;
        }
    }
}