using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Aspects.Components.Caching;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace IvorySharp.Aspects.Components.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal class MethodAspectDependencyInjector : IMethodAspectDependencyInjector
    {
        private IDependencyProvider _dependencyProvider;
        private Func<Type, AspectPropertyDependency[]> _cachedPropertyDependencyProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodAspectDependencyInjector"/>.
        /// </summary>
        /// <param name="dependencyProvider">Провайдер сервисов.</param>
        public MethodAspectDependencyInjector(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _cachedPropertyDependencyProvider = Cache.CreateProducer<Type, AspectPropertyDependency[]>(GetPropertyDependencies);
        }

        /// <inheritdoc />
        public void InjectPropertyDependencies(MethodAspect aspect)
        {
            foreach (var propertyDependency in _cachedPropertyDependencyProvider(aspect.GetType()))
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

                propertyDependency.FastPropertySetter(aspect, service);
            }
        }

        /// <inheritdoc />
        public AspectPropertyDependency[] GetPropertyDependencies(Type aspectType)
        {
            var dependencies = new List<AspectPropertyDependency>();
            var properties = aspectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanWrite || property.GetSetMethod(nonPublic: false) == null)
                    continue;

                var aspectDependency = property.GetCustomAttribute<InjectDependencyAttribute>(inherit: false);
                if (aspectDependency == null)
                    continue;

                dependencies.Add(new AspectPropertyDependency(aspectDependency, property));
            }

            return dependencies.ToArray();
        }
    }
}