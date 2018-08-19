using IvorySharp.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal sealed class AspectDependencyInjector : IAspectDependencyInjector
    {
        private readonly IComponentProvider<IDependencyProvider> _dependencyProvider;
        private readonly IComponentProvider<IAspectDependencyProvider>  _aspectDependencyProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectDependencyInjector"/>.
        /// </summary>
        /// <param name="dependencyProvider">Провайдер зависимостей.</param>
        /// <param name="aspectDependencyProvider">Провайдер зависимостей аспекта.</param>
        public AspectDependencyInjector(
            IComponentProvider<IDependencyProvider> dependencyProvider, 
            IComponentProvider<IAspectDependencyProvider> aspectDependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _aspectDependencyProvider = aspectDependencyProvider;
        }

        /// <inheritdoc />
        public void InjectPropertyDependencies(MethodAspect aspect)
        {
            var aspectDependencyProvider = _aspectDependencyProvider.Get();
            var dependencyProvider = _dependencyProvider.Get();
            
            foreach (var propertyDependency in aspectDependencyProvider.GetPropertyDependencies(aspect.GetType()))
            {
                object service;

                if (propertyDependency.Dependency.Transparent)
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? dependencyProvider.GetTransparentService(propertyDependency.Property.PropertyType)
                        : dependencyProvider.GetTransparentNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }
                else
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? dependencyProvider.GetService(propertyDependency.Property.PropertyType)
                        : dependencyProvider.GetNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }

                propertyDependency.FastPropertySetter(aspect, service);
            }
        }
    }
}