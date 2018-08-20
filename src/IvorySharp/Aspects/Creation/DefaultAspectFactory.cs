using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Инициализатор аспектов.
    /// </summary>
    internal sealed class DefaultAspectFactory : IAspectFactory
    {
        private readonly IComponentProvider<IAspectsPreInitializer> _preInitializerProvider;
        private readonly IComponentProvider<IAspectDependencyInjector> _dependencyInjectorProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="DefaultAspectFactory"/>.
        /// </summary>
        public DefaultAspectFactory(
            IComponentProvider<IAspectsPreInitializer> preInitializerProvider,
            IComponentProvider<IAspectDependencyInjector> dependencyInjectorProvider)
        {
            _preInitializerProvider = preInitializerProvider;
            _dependencyInjectorProvider = dependencyInjectorProvider;
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationContext context)
        {
            var aspects = _preInitializerProvider.Get().PrepareBoundaryAspects(context);
            var dependencyInjector = _dependencyInjectorProvider.Get();
            
            foreach (var aspect in aspects)
            {
                dependencyInjector.InjectPropertyDependencies(aspect);
                aspect.Initialize();
            }

            return aspects;
        }

        /// <inheritdoc />
        public MethodInterceptionAspect CreateInterceptionAspect(IInvocationContext context)
        {
            var aspect = _preInitializerProvider.Get().PrepareInterceptAspect(context);
            var dependencyInjector = _dependencyInjectorProvider.Get();
            
            dependencyInjector.InjectPropertyDependencies(aspect);
            aspect.Initialize();

            return aspect;
        }
    }
}