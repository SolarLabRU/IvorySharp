using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Набор компонентов по умолчанию.
    /// </summary>
    internal class DefaultComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public IDependencyProvider DependencyProvider { get; }

        /// <inheritdoc />
        public IAspectSelector AspectSelector => _aspectSelectorProvider.Value;
        private readonly Lazy<IAspectSelector> _aspectSelectorProvider;

        /// <inheritdoc />
        public IAspectWeavePredicate AspectWeavePredicate => _aspectWeavePredicateProvider.Value;
        private readonly Lazy<IAspectWeavePredicate> _aspectWeavePredicateProvider;

        /// <inheritdoc />
        public IAspectDeclarationCollector AspectDeclarationCollector => _aspectDeclarationCollectorProvider.Value;
        private readonly Lazy<IAspectDeclarationCollector> _aspectDeclarationCollectorProvider;

        /// <inheritdoc />
        public IPipelineExecutor AspectPipelineExecutor => _aspectPipelineExecutorProvider.Value;
        private readonly Lazy<IPipelineExecutor> _aspectPipelineExecutorProvider;

        /// <inheritdoc />
        public IAspectFactory AspectFactory => _aspectFactoryProvider.Value;
        private readonly Lazy<IAspectFactory> _aspectFactoryProvider;

        /// <inheritdoc />
        public IAspectDependencyInjector AspectDependencyInjector => _aspectDependencyInjectorProvider.Value;
        private readonly Lazy<IAspectDependencyInjector> _aspectDependencyInjectorProvider;

        /// <inheritdoc />
        public IAspectOrderStrategy AspectOrderStrategy => _aspectOrderStrategyProvider.Value;
        private readonly Lazy<IAspectOrderStrategy> _aspectOrderStrategyProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="DefaultComponentsStore"/>.
        /// </summary>
        public DefaultComponentsStore(IDependencyProvider dependencyProvider)
        {
            DependencyProvider = dependencyProvider;
            
            _aspectSelectorProvider = new Lazy<IAspectSelector>(() => new DefaultAspectSelector());
            _aspectWeavePredicateProvider = new Lazy<IAspectWeavePredicate>(
                () => new CachedWeavePredicate(new DeclaringTypeWeavePredicate(AspectSelector)));
            
            _aspectDeclarationCollectorProvider = new Lazy<IAspectDeclarationCollector>(
                () => new DeclaringTypeAspectDeclarationCollector(AspectSelector));
            
            _aspectPipelineExecutorProvider = new Lazy<IPipelineExecutor>(
                () => AspectInvocationPipelineExecutor.Instance);
            
            _aspectFactoryProvider = new Lazy<IAspectFactory>(
                () => new AspectFactory(AspectDeclarationCollector, AspectDependencyInjector, AspectOrderStrategy));
            
            _aspectDependencyInjectorProvider = new Lazy<IAspectDependencyInjector>(
                () => new AspectDependencyInjector(dependencyProvider));
            
            _aspectOrderStrategyProvider = new Lazy<IAspectOrderStrategy>(
                () => new DefaultAspectOrderStrategy());
        }

    }
}