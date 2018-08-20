using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Caching;

namespace IvorySharp.Components
{
    /// <summary>
    /// Компоненты по умолчанию.
    /// </summary>
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
            
            var aspectsPreInitializerProvider = new LazyComponentProvider<IAspectsPreInitializer>(
                () => new CachedAspectsPreInitializer(
                    new DefaultAspectsPreInitializer(AspectDeclarationCollector, AspectOrderStrategy)));
            
            AspectFactory = new LazyComponentProvider<IAspectFactory>(
                () => new DefaultAspectFactory(aspectsPreInitializerProvider, AspectDependencyInjector));
            
            var selectorProvider = new LazyComponentProvider<IAspectDependencySelector>(
                () => new CachedAspectDependencySelector(
                    new DefaultAspectDependencySelector()));
            
            AspectDependencyInjector = new LazyComponentProvider<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyProvider, selectorProvider));
            
            AspectOrderStrategy = new LazyComponentProvider<IAspectOrderStrategy>(
                () => new DefaultAspectOrderStrategy());
        }
    }
}