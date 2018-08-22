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
        public IComponentHolder<IDependencyProvider> DependencyHolder { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectSelector> AspectSelector { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectWeavePredicate> AspectWeavePredicate { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectDeclarationCollector> AspectDeclarationCollector { get; }

        /// <inheritdoc />
        public IComponentHolder<IInvocationPipelineFactory> PipelineFactory { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectFactory> AspectFactory { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectOrderStrategy> AspectOrderStrategy { get; }

        internal DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyHolder = dependencyProvider.ToProvider();
            
            AspectSelector = new LazyComponentHolder<IAspectSelector>(() => new DefaultAspectSelector());
            AspectWeavePredicate = new LazyComponentHolder<IAspectWeavePredicate>(
                () => new CachedWeavePredicate(
                    new DeclaringTypeWeavePredicate(AspectSelector)));
            
            AspectDeclarationCollector = new LazyComponentHolder<IAspectDeclarationCollector>(
                () => new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new LazyComponentHolder<IInvocationPipelineFactory>(
                () => new AsyncDeterminingPipelineFactory(MethodCache.Instance));
            
            var aspectsPreInitializerProvider = new LazyComponentHolder<IAspectsPreInitializer>(
                () => new CachedAspectsPreInitializer(
                    new DefaultAspectsPreInitializer(AspectDeclarationCollector, AspectOrderStrategy)));
            
            AspectFactory = new LazyComponentHolder<IAspectFactory>(
                () => new DefaultAspectFactory(aspectsPreInitializerProvider, AspectDependencyInjector));
            
            var selectorProvider = new LazyComponentHolder<IAspectDependencySelector>(
                () => new CachedAspectDependencySelector(
                    new DefaultAspectDependencySelector()));
            
            AspectDependencyInjector = new LazyComponentHolder<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyHolder, selectorProvider));
            
            AspectOrderStrategy = new LazyComponentHolder<IAspectOrderStrategy>(
                () => new DefaultAspectOrderStrategy());
        }
    }
}