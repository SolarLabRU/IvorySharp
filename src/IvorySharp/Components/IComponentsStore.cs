using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using JetBrains.Annotations;

namespace IvorySharp.Components
{
    /// <summary>
    /// A store of library components that can be replaced.
    /// </summary>
    [PublicAPI]
    public interface IComponentsStore 
    { 
        /// <summary>
        /// Service dependency provider. 
        /// </summary>
        IComponentHolder<IDependencyProvider> DependencyHolder { get; }

        /// <summary>
        /// Aspect selection stategy.
        /// </summary>
        IComponentHolder<IAspectSelector> AspectSelector { get; }

        /// <summary>
        /// Aspect weave predicate. 
        /// </summary>
        IComponentHolder<IAspectWeavePredicate> AspectWeavePredicate { get; }

        /// <summary>
        /// Aspect declarations collector.
        /// </summary>
        IComponentHolder<IAspectDeclarationCollector> AspectDeclarationCollector { get; }

        /// <summary>
        /// Common pipeline components factory.
        /// </summary>
        IComponentHolder<IInvocationPipelineFactory> PipelineFactory { get; }

        /// <summary>
        /// Компонент, выпонялющий инициализацию аспектов.
        /// </summary>
        IComponentHolder<IAspectFactory> AspectFactory { get; }

        /// <summary>
        /// Aspect dependency injector.
        /// </summary>
        IComponentHolder<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <summary>
        /// Aspect ordering strategy.
        /// </summary>
        IComponentHolder<IAspectOrderStrategy> AspectOrderStrategy { get; }
        
        /// <summary>
        /// Factory that creates data required for aspect weaving. 
        /// </summary>
        IComponentHolder<IInvocationWeaveDataProviderFactory> WeaveDataProviderFactory { get; }
        
        /// <summary>
        /// Aspect finalizer.
        /// </summary>
        IComponentHolder<IAspectFinalizer> AspectFinalizer { get; }
    }
}