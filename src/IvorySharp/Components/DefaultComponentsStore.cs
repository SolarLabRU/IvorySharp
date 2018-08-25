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
            DependencyHolder = dependencyProvider.ToInstanceHolder();
            
            AspectSelector = new InstanceComponentHolder<IAspectSelector>(new DefaultAspectSelector());
            AspectWeavePredicate = new InstanceComponentHolder<IAspectWeavePredicate>(
                new DeclaringTypeWeavePredicate(AspectSelector, ConcurrentDictionaryCacheFactory.Default));
            
            AspectDeclarationCollector = new InstanceComponentHolder<IAspectDeclarationCollector>(
                new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new InstanceComponentHolder<IInvocationPipelineFactory>(
                new AsyncDeterminingPipelineFactory(MethodInfoCache.Instance));
            
            AspectOrderStrategy = new InstanceComponentHolder<IAspectOrderStrategy>(
                new DefaultAspectOrderStrategy());
            
            var aspectDependencySelectorHolder = new InstanceComponentHolder<IAspectDependencySelector>(
                new CachedAspectDependencySelector(
                    new DefaultAspectDependencySelector(),
                    CacheDelegateFactory<ConcurrentDictionaryCacheFactory>.Instance));
            
            var aspectsPreInitializerHolder = new InstanceComponentHolder<IAspectsPreInitializer>(
                new CachedAspectsPreInitializer(
                    new DefaultAspectsPreInitializer(
                        AspectDeclarationCollector, AspectOrderStrategy, aspectDependencySelectorHolder),
                    CacheDelegateFactory<ConcurrentDictionaryCacheFactory>.Instance));
            
            AspectDependencyInjector = new LazyComponentHolder<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyHolder, aspectDependencySelectorHolder));
            
            AspectFactory = new InstanceComponentHolder<IAspectFactory>(
                new DefaultAspectFactory(aspectsPreInitializerHolder, AspectDependencyInjector));
        }
    }
}