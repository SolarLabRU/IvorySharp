using System;
using System.Reflection;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Информация о зависимости аспекта.
    /// </summary>
    public class AspectPropertyDependency
    {
        /// <summary>
        /// Атрибут зависимости.
        /// </summary>
        public DependencyAttribute Dependency { get; }

        /// <summary>
        /// Информация о свойстве, на котором задан атрибут зависимости.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Быстрый сеттер свойства.
        /// </summary>
        public Action<object, object> FastPropertySetter { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectPropertyDependency"/>.
        /// </summary>
        /// <param name="dependency">Атрибут зависимости аспекта.</param>
        /// <param name="property">Свойство, на котором размещен атрибут.</param>
        public AspectPropertyDependency(DependencyAttribute dependency, PropertyInfo property)
        {
            Dependency = dependency;
            Property = property;
            FastPropertySetter = Expressions.CreatePropertySetter(property);
        }
    }
}