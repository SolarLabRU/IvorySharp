using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Фабрика аспектов.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    internal sealed class DefaultAspectFactory<TAspect> : IAspectFactory<TAspect> 
        where TAspect : OrderableMethodAspect
    {
        private readonly IComponentProvider<IAspectDependencyInjector> _dependencyInjectorProvider;
        private readonly IComponentProvider<IAspectPreInitializer<TAspect>> _preInitializerProvider;

        /// <summary>
        /// Инициализирует фабрику аспектов.
        /// </summary>
        public DefaultAspectFactory(
            IComponentProvider<IAspectDependencyInjector> dependencyInjectorProvider,
            IComponentProvider<IAspectPreInitializer<TAspect>> preInitializerProvider)
        {
            _dependencyInjectorProvider = dependencyInjectorProvider;
            _preInitializerProvider = preInitializerProvider;
        }

        /// <inheritdoc />
        public TAspect[] CreateAspects(IInvocationContext context)
        {
            var preInitializer = _preInitializerProvider.Get();
            var dependencyInjector = _dependencyInjectorProvider.Get();
            var aspects = preInitializer.PrepareAspects(context);
            
            foreach (var aspect in aspects)
            {
                dependencyInjector.InjectPropertyDependencies(aspect);
                aspect.Initialize();
            }

            return aspects;
        }
    }
}