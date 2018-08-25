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

        private IAspectsPreInitializer _preInitializer;
        private IAspectDependencyInjector _dependencyInjector;
        
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
            if (_preInitializer == null)
                _preInitializer = _preInitializerHolder.Get();
            
            var aspects = _preInitializer.PrepareBoundaryAspects(context);
   
            foreach (var aspect in aspects)
            {
                if (aspect.HasDependencies)
                {
                    if (_dependencyInjector == null)
                        _dependencyInjector = _dependencyInjectorHolder.Get();
                    
                    _dependencyInjector.InjectPropertyDependencies(aspect);
                }
                
                aspect.Initialize();
            }

            return aspects;
        }

        /// <inheritdoc />
        public MethodInterceptionAspect CreateInterceptionAspect(IInvocationContext context)
        {
            var aspect = _preInitializerHolder.Get().PrepareInterceptAspect(context);

            if (aspect.HasDependencies)
            {
                if (_dependencyInjector == null)
                    _dependencyInjector = _dependencyInjectorHolder.Get();
                
                _dependencyInjector.InjectPropertyDependencies(aspect);
            }
            
            aspect.Initialize();

            return aspect;
        }
    }
}