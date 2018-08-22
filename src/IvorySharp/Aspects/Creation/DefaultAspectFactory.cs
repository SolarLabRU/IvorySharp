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
        private readonly IComponentHolder<IAspectsPreInitializer> _preInitializerHolder;
        private readonly IComponentHolder<IAspectDependencyInjector> _dependencyInjectorHolder;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="DefaultAspectFactory"/>.
        /// </summary>
        public DefaultAspectFactory(
            IComponentHolder<IAspectsPreInitializer> preInitializerHolder,
            IComponentHolder<IAspectDependencyInjector> dependencyInjectorHolder)
        {
            _preInitializerHolder = preInitializerHolder;
            _dependencyInjectorHolder = dependencyInjectorHolder;
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationContext context)
        {
            var aspects = _preInitializerHolder.Get().PrepareBoundaryAspects(context);
            var dependencyInjector = _dependencyInjectorHolder.Get();
            
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
            var aspect = _preInitializerHolder.Get().PrepareInterceptAspect(context);
            var dependencyInjector = _dependencyInjectorHolder.Get();
            
            dependencyInjector.InjectPropertyDependencies(aspect);
            aspect.Initialize();

            return aspect;
        }
    }
}