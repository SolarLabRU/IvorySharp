using IvorySharp.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal sealed class AspectDependencyInjector : IAspectDependencyInjector
    {
        private readonly IComponentHolder<IDependencyProvider> _dependencyProviderHolder;
        private readonly IComponentHolder<IAspectDependencySelector> _dependencySelectorHolder;

        private IDependencyProvider _dependencyProvider;
        private IAspectDependencySelector _dependencySelector;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectDependencyInjector"/>.
        /// </summary>
        public AspectDependencyInjector(
            IComponentHolder<IDependencyProvider> dependencyProviderHolder,
            IComponentHolder<IAspectDependencySelector> dependencySelectorHolder)
        {
            _dependencyProviderHolder = dependencyProviderHolder;
            _dependencySelectorHolder = dependencySelectorHolder;
        }
        
        /// <inheritdoc />
        public void InjectPropertyDependencies(MethodAspect aspect)
        {
            if (_dependencyProvider == null)
                _dependencyProvider = _dependencyProviderHolder.Get();

            if (_dependencySelector == null)
                _dependencySelector = _dependencySelectorHolder.Get();
            
            foreach (var propertyDependency in _dependencySelector.SelectPropertyDependencies(aspect.GetType()))
            {
                object service;

                if (propertyDependency.Dependency.Transparent)
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? _dependencyProvider.GetTransparentService(propertyDependency.Property.PropertyType)
                        : _dependencyProvider.GetTransparentNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }
                else
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? _dependencyProvider.GetService(propertyDependency.Property.PropertyType)
                        : _dependencyProvider.GetNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }

                propertyDependency.PropertySetter(aspect, service);
            }
        }
    }
}