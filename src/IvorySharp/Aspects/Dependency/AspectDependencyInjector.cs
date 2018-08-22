using IvorySharp.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal sealed class AspectDependencyInjector : IAspectDependencyInjector
    {
        private readonly IComponentHolder<IDependencyProvider> _dependencyHolder;
        private readonly IComponentHolder<IAspectDependencySelector> _dependencySelector;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectDependencyInjector"/>.
        /// </summary>
        public AspectDependencyInjector(
            IComponentHolder<IDependencyProvider> dependencyHolder,
            IComponentHolder<IAspectDependencySelector> dependencySelector)
        {
            _dependencyHolder = dependencyHolder;
            _dependencySelector = dependencySelector;
        }
        
        /// <inheritdoc />
        public void InjectPropertyDependencies(MethodAspect aspect)
        {
            var selector = _dependencySelector.Get();
            var provider = _dependencyHolder.Get();
            
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