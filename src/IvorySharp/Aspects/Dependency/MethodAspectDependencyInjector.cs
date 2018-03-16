using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Компонент для внедрения зависимостей в аспекты.
    /// </summary>
    internal class MethodAspectDependencyInjector
    {
        private IServiceProvider _serviceProvider;
        private Func<Type, AspectPropertyDependency[]> _aspectDependencyProvider;

        public MethodAspectDependencyInjector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _aspectDependencyProvider = Memoizer.Memoize<Type, AspectPropertyDependency[]>(GetAspectPropertyDependencies);
        }

        /// <summary>
        /// Выполняет внедрение зависимостей в аспект.
        /// </summary>
        /// <param name="aspect">Модель аспекта.</param>
        public void InjectDependencies(MethodAspect aspect)
        {
            if (!aspect.HasDependencies)
                return;
            
            foreach (var propertyDependency in _aspectDependencyProvider(aspect.GetType()))
            {
                object service;
                
                if (propertyDependency.Dependency.Transparent)
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? _serviceProvider.GetTransparentService(propertyDependency.Property.PropertyType)
                        : _serviceProvider.GetTransparentNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }
                else
                {
                    service = propertyDependency.Dependency.ServiceKey == null
                        ? _serviceProvider.GetService(propertyDependency.Property.PropertyType)
                        : _serviceProvider.GetNamedService(
                            propertyDependency.Property.PropertyType, propertyDependency.Dependency.ServiceKey);
                }
                
                propertyDependency.PropertySetter(aspect, service);
            }
        }

        /// <summary>
        /// Получает информацию о зависимостях на типе аспекта.
        /// </summary>
        /// <param name="aspectType">Тип аспекта.</param>
        /// <returns>Информация о зависимостях аспекта.</returns>
        private AspectPropertyDependency[] GetAspectPropertyDependencies(Type aspectType)
        {
            var dependencies = new List<AspectPropertyDependency>();
            var properties = aspectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                var aspectDependency = property.GetCustomAttribute<InjectDependencyAttribute>(inherit: false);
                if (aspectDependency == null)
                    continue;
                
                if (!property.CanWrite || property.GetSetMethod(nonPublic: false) == null)
                    continue;
                
                dependencies.Add(new AspectPropertyDependency(aspectDependency, property));
            }

            return dependencies.ToArray();
        }
        
        /// <summary>
        /// Информация о зависимости аспекта.
        /// </summary>
        private struct AspectPropertyDependency
        {
            /// <summary>
            /// Атрибут зависимости.
            /// </summary>
            public readonly InjectDependencyAttribute Dependency;
            
            /// <summary>
            /// Информация о свойстве, на котором задан атрибут зависимости.
            /// </summary>
            public readonly PropertyInfo Property;
            
            /// <summary>
            /// Быстрый сеттер свойства.
            /// </summary>
            public readonly Action<object, object> PropertySetter;

            public AspectPropertyDependency(InjectDependencyAttribute dependency, PropertyInfo property)
            {
                Dependency = dependency;
                Property = property;
                PropertySetter = Expressions.CreatePropertySetter(property);
            }
        }
    }
}