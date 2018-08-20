using IvorySharp.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal sealed class AspectDependencyInjector : IAspectDependencyInjector
    {
        private readonly IComponentProvider<IDependencyProvider> _dependencyProvider;
        private readonly IComponentProvider<IAspectDependencySelector> _dependencySelector;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectDependencyInjector"/>.
        /// </summary>
        public AspectDependencyInjector(
            IComponentProvider<IDependencyProvider> dependencyProvider,
            IComponentProvider<IAspectDependencySelector> dependencySelector)
        {
            _dependencyProvider = dependencyProvider;
            _dependencySelector = dependencySelector;
        }
        
        /// <inheritdoc />
        public void InjectPropertyDependencies(MethodAspect aspect)
        {
            var selector = _dependencySelector.Get();
            var provider = _dependencyProvider.Get();
            
            foreach (var propertyDependency in selector.SelectPropertyDependencies(aspect.GetType()))
            {
                object service;

                if (propertyDependency.Dependency.Transparent)
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? provider.GetTransparentService(propertyDependency.Property.PropertyType)
                        : provider.GetTransparentNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }
                else
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? provider.GetService(propertyDependency.Property.PropertyType)
                        : provider.GetNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }

                propertyDependency.FastPropertySetter(aspect, service);
            }
        }
    }
}