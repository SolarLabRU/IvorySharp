using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

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