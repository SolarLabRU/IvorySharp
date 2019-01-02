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
    /// A set of default library components.
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

        /// <summary>
        /// Creates a new instance of <see cref="DefaultComponentsStore"/>.
        /// </summary>
        /// <param name="dependencyProvider">Dependency provider.</param>
        internal DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyHolder = dependencyProvider.ToInstanceHolder();
            
            AspectFinalizer = new InstanceComponentHolder<IAspectFinalizer>(new DisposeAspectFinalizer());  
            AspectSelector = new InstanceComponentHolder<IAspectSelector>(new AspectSelector());
            AspectWeavePredicate = new InstanceComponentHolder<IAspectWeavePredicate>(
                new DeclaringTypeWeavePredicate(AspectSelector));
            
            AspectDeclarationCollector = new InstanceComponentHolder<IAspectDeclarationCollector>(
                new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new InstanceComponentHolder<IInvocationPipelineFactory>(
                new AsyncDeterminingPipelineFactory(
                    MethodInfoCache.Instance, ConcurrentDictionaryCacheFactory.Default));
            
            AspectOrderStrategy = new InstanceComponentHolder<IAspectOrderStrategy>(
                new AspectOrderStrategy());
            
            var aspectDependencySelectorHolder = new InstanceComponentHolder<IAspectDependencySelector>(
                new CachedAspectDependencySelector(
                    new AspectDependencySelector(),
                    CacheDelegateFactory<ConcurrentDictionaryCacheFactory>.Instance));
            
            AspectFactory = new InstanceComponentHolder<IAspectFactory>(
                new AspectFactory(
                    AspectDeclarationCollector, 
                    AspectOrderStrategy, 
                    aspectDependencySelectorHolder,
                    AspectFinalizer));
            
            AspectDependencyInjector = new LazyComponentHolder<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyHolder, aspectDependencySelectorHolder));
            
            WeaveDataProviderFactory = new InstanceComponentHolder<IInvocationWeaveDataProviderFactory>(
                new InvocationWeaveDataProviderFactory(
                    AspectWeavePredicate, AspectFactory, PipelineFactory, 
                    ConcurrentDictionaryCacheFactory.Default));    
        }
    }
}