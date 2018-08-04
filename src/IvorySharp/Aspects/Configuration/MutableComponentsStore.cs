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
        public IAspectSelector AspectSelector { get; set; }

        /// <inheritdoc />
        public IAspectWeavePredicate AspectWeavePredicate { get; set; }

        /// <inheritdoc />
        public IAspectDeclarationCollector AspectDeclarationCollector { get; set; }

        /// <inheritdoc />
        public IPipelineExecutor AspectPipelineExecutor { get; set; }

        /// <inheritdoc />
        public IAspectFactory AspectFactory { get; set; }

        /// <inheritdoc />
        public IAspectDependencyInjector AspectDependencyInjector { get; set; }

        /// <inheritdoc />
        public IAspectOrderStrategy AspectOrderStrategy { get; set; }
    }
}