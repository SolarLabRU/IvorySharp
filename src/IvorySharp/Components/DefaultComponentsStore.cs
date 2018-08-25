using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
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

        /// <inheritdoc />
        public IComponentHolder<IInvocationWeaveDataProviderFactory> WeaveDataProviderFactory { get; }

        /// <inheritdoc />
        public IComponentHolder<IAspectFinalizer> AspectFinalizer { get; }

        internal DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyHolder = dependencyProvider.ToInstanceHolder();
            
            AspectSelector = new InstanceComponentHolder<IAspectSelector>(new AspectSelector());
            AspectWeavePredicate = new InstanceComponentHolder<IAspectWeavePredicate>(
                new DeclaringTypeWeavePredicate(AspectSelector));
            
            AspectDeclarationCollector = new InstanceComponentHolder<IAspectDeclarationCollector>(
                new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new InstanceComponentHolder<IInvocationPipelineFactory>(
                new AsyncDeterminingPipelineFactory(MethodInfoCache.Instance));
            
            AspectOrderStrategy = new InstanceComponentHolder<IAspectOrderStrategy>(
                new AspectOrderStrategy());
            
            var aspectDependencySelectorHolder = new InstanceComponentHolder<IAspectDependencySelector>(
                new CachedAspectDependencySelector(
                    new AspectDependencySelector(),
                    CacheDelegateFactory<ConcurrentDictionaryCacheFactory>.Instance));
            
            AspectFactory = new InstanceComponentHolder<IAspectFactory>(
                new AspectFactory(
                    AspectDeclarationCollector, AspectOrderStrategy, aspectDependencySelectorHolder));
            
            AspectDependencyInjector = new LazyComponentHolder<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyHolder, aspectDependencySelectorHolder));
            
            WeaveDataProviderFactory = new InstanceComponentHolder<IInvocationWeaveDataProviderFactory>(
                new InvocationWeaveDataProviderFactory(
                    AspectWeavePredicate, AspectFactory, PipelineFactory, 
                    MethodInfoCache.Instance, ConcurrentDictionaryCacheFactory.Default));    
            
            AspectFinalizer = new InstanceComponentHolder<IAspectFinalizer>(new DisposeAspectFinalizer());
        }
    }
}