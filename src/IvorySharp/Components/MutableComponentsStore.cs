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

        /// <summary>
        /// Замораживает компоненты (запрещает замену внутренней реализации).
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
        }
    }
}