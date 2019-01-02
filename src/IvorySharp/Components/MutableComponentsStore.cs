using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

namespace IvorySharp.Components
{
    /// <summary>
    /// Represents components store that can be modified.
    /// </summary>
    internal sealed class MutableComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public IComponentHolder<IDependencyProvider> DependencyHolder { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectSelector> AspectSelector { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectWeavePredicate> AspectWeavePredicate { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectDeclarationCollector> AspectDeclarationCollector { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IInvocationPipelineFactory> PipelineFactory { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectFactory> AspectFactory { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectDependencyInjector> AspectDependencyInjector { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectOrderStrategy> AspectOrderStrategy { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IInvocationWeaveDataProviderFactory> WeaveDataProviderFactory { get; set; }

        /// <inheritdoc />
        public IComponentHolder<IAspectFinalizer> AspectFinalizer { get; set; }

        /// <summary>
        /// Freezes components (prohibits assignment of components inside component holders).
        /// </summary>
        public void Freeze()
        {
            DependencyHolder.Freeze();
            AspectSelector.Freeze();
            AspectWeavePredicate.Freeze();
            AspectDeclarationCollector.Freeze();
            PipelineFactory.Freeze();
            AspectFactory.Freeze();
            AspectDependencyInjector.Freeze();
            AspectOrderStrategy.Freeze();
            WeaveDataProviderFactory.Freeze();
            AspectFinalizer.Freeze();
        }
    }
}