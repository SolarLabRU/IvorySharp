using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Caching;
using IvorySharp.Components;

namespace IvorySharp.Aspects.Configuration
{
    internal sealed class DefaultComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public IComponentProvider<IDependencyProvider> DependencyProvider { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectSelector> AspectSelector { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectWeavePredicate> AspectWeavePredicate { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectDeclarationCollector> AspectDeclarationCollector { get; }

        /// <inheritdoc />
        public IComponentProvider<IInvocationPipelineFactory> PipelineFactory { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectFactory> AspectFactory { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectOrderStrategy> AspectOrderStrategy { get; }

        internal DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyProvider = dependencyProvider.ToProvider();
            
            AspectSelector = new LazyComponentProvider<IAspectSelector>(() => new DefaultAspectSelector());
            AspectWeavePredicate = new LazyComponentProvider<IAspectWeavePredicate>(
                () => new CachedWeavePredicate(
                    new DeclaringTypeWeavePredicate(AspectSelector)));
            
            AspectDeclarationCollector = new LazyComponentProvider<IAspectDeclarationCollector>(
                () => new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new LazyComponentProvider<IInvocationPipelineFactory>(
                () => new AsyncDeterminingPipelineFactory(MethodCache.Instance));
            
            AspectFactory = new LazyComponentProvider<IAspectFactory>(
                () => new AspectFactory(
                    AspectDeclarationCollector, 
                    AspectDependencyInjector,
                    AspectOrderStrategy));
            
            AspectDependencyInjector = new LazyComponentProvider<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(dependencyProvider));
            
            AspectOrderStrategy = new LazyComponentProvider<IAspectOrderStrategy>(
                () => new DefaultAspectOrderStrategy());
        }
    }
}