using System.Reflection;
using IvorySharp.Linq;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Информация о зависимости аспекта.
    /// </summary>
    [PublicAPI]
    public sealed class AspectPropertyDependency
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
        /// Сеттер свойства.
        /// </summary>
        public PropertySetter PropertySetter { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectPropertyDependency"/>.
        /// </summary>
        /// <param name="dependency">Атрибут зависимости аспекта.</param>
        /// <param name="property">Свойство, на котором размещен атрибут.</param>
        public AspectPropertyDependency(DependencyAttribute dependency, PropertyInfo property)
        {
            Dependency = dependency;
            Property = property;
            PropertySetter = Expressions.CreatePropertySetter(property);
        }
    }
}