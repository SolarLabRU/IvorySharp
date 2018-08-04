using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Компоненты библиотеки.
    /// </summary>
    internal class MutableComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public IDependencyProvider DependencyProvider { get; set; }

        /// <inheritdoc />
        public IMethodAspectSelectionStrategy AspectSelectionStrategy { get; set; }

        /// <inheritdoc />
        public IMethodAspectWeavePredicate AspectWeavePredicate { get; set; }

        /// <inheritdoc />
        public IMethodAspectDeclarationCollector AspectDeclarationCollector { get; set; }

        /// <inheritdoc />
        public IMethodAspectPipelineExecutor AspectPipelineExecutor { get; set; }

        /// <inheritdoc />
        public IMethodAspectInitializer AspectInitializer { get; set; }

        /// <inheritdoc />
        public IMethodAspectDependencyInjector AspectDependencyInjector { get; set; }

        /// <inheritdoc />
        public IMethodAspectOrderStrategy AspectOrderStrategy { get; set; }
    }
}