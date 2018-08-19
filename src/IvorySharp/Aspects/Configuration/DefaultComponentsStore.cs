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
        public IComponentProvider<IAspectFactory<MethodBoundaryAspect>> BoundaryAspectFactory { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectFactory<MethodInterceptionAspect>> InterceptionAspectFactory { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <inheritdoc />
        public IComponentProvider<IAspectOrderStrategy> AspectOrderStrategy { get; }

        internal DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyProvider = dependencyProvider.ToProvider();
            
            AspectSelector = new LazyComponentProvider<IAspectSelector>(() => new DefaultAspectSelector());
            
            var declaringTypeWeavePredicate = new LazyComponentProvider<IAspectWeavePredicate>(
                () => new DeclaringTypeWeavePredicate(AspectSelector));
            
            AspectWeavePredicate = new LazyComponentProvider<IAspectWeavePredicate>(
                () => new CachedWeavePredicate(declaringTypeWeavePredicate));
            
            AspectDeclarationCollector = new LazyComponentProvider<IAspectDeclarationCollector>(
                () => new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            PipelineFactory = new LazyComponentProvider<IInvocationPipelineFactory>(
                () => new AsyncDeterminingPipelineFactory(MethodCache.Instance));
            
            var defaultBoundaryAspectPreinitializer = 
                new LazyComponentProvider<IAspectPreInitializer<MethodBoundaryAspect>>(
                    () => new DefaultAspectPreInitializer<MethodBoundaryAspect>(
                        AspectDeclarationCollector, 
                        AspectOrderStrategy));
            
            var cachedBoundaryAspectPreinitializer 
                = new LazyComponentProvider<IAspectPreInitializer<MethodBoundaryAspect>>(
                    () => new CachedAspectPreInitializer<MethodBoundaryAspect>(
                        defaultBoundaryAspectPreinitializer));

            BoundaryAspectFactory = new LazyComponentProvider<IAspectFactory<MethodBoundaryAspect>>(
                () => new DefaultAspectFactory<MethodBoundaryAspect>(
                    AspectDependencyInjector,
                    cachedBoundaryAspectPreinitializer));

            var defaultInterceptionAspectPreinitializer = 
                new LazyComponentProvider<IAspectPreInitializer<MethodInterceptionAspect>>(
                    () => new DefaultAspectPreInitializer<MethodInterceptionAspect>(
                        AspectDeclarationCollector, 
                        AspectOrderStrategy));
            
            var cachedInterceptionAspectPreinitializer 
                = new LazyComponentProvider<IAspectPreInitializer<MethodInterceptionAspect>>(
                    () => new CachedAspectPreInitializer<MethodInterceptionAspect>(
                        defaultInterceptionAspectPreinitializer));
            
            InterceptionAspectFactory = new LazyComponentProvider<IAspectFactory<MethodInterceptionAspect>>(
                () => new MethodInterceptionAspectFactory(
                    AspectDependencyInjector, 
                    cachedInterceptionAspectPreinitializer));
            
            var defaultAspectDependencyProvider = new LazyComponentProvider<IAspectDependencyProvider>(
                () => new DefaultAspectDependencyProvider());
            
            var cachedAspectDependencyProvider = new LazyComponentProvider<IAspectDependencyProvider>(
                () => new CachedAspectDependencyProvider(defaultAspectDependencyProvider));
            
            AspectDependencyInjector = new LazyComponentProvider<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(DependencyProvider, cachedAspectDependencyProvider));
            
            AspectOrderStrategy = new LazyComponentProvider<IAspectOrderStrategy>(
                () => new DefaultAspectOrderStrategy());
        }
    }
}