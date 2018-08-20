using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

namespace IvorySharp.Components
{
    /// <summary>
    /// Компоненты библиотеки.
    /// </summary>
    internal sealed class MutableComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public IComponentProvider<IDependencyProvider> DependencyProvider { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectSelector> AspectSelector { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectWeavePredicate> AspectWeavePredicate { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectDeclarationCollector> AspectDeclarationCollector { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IInvocationPipelineFactory> PipelineFactory { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectFactory> AspectFactory { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectDependencyInjector> AspectDependencyInjector { get; set; }

        /// <inheritdoc />
        public IComponentProvider<IAspectOrderStrategy> AspectOrderStrategy { get; set; }

        /// <summary>
        /// Замораживает компоненты (запрещает замену внутренней реализации).
        /// </summary>
        public void Freeze()
        {
            DependencyProvider.Freeze();
            AspectSelector.Freeze();
            AspectWeavePredicate.Freeze();
            AspectDeclarationCollector.Freeze();
            PipelineFactory.Freeze();
            AspectFactory.Freeze();
            AspectDependencyInjector.Freeze();
            AspectOrderStrategy.Freeze();
        }
    }
}