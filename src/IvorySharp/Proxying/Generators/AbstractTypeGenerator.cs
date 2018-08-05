using System.Reflection;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Generators
{
    /// <summary>
    /// Базовый класс для генерации типа.
    /// </summary>
    internal abstract class AbstractTypeGenerator
    {
        /// <summary>
        /// Динамический тип.
        /// </summary>
        protected readonly TypeBuilder DynamicTypeBuilder;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AbstractTypeGenerator"/>.
        /// </summary>
        /// <param name="dynamicTypeBuilder">Динамический тип.</param>
        protected AbstractTypeGenerator(TypeBuilder dynamicTypeBuilder)
        {
            DynamicTypeBuilder = dynamicTypeBuilder;
        }

        /// <summary>
        /// Генерирует динамический тип.
        /// </summary>
        /// <returns>Тип.</returns>
        public abstract TypeInfo Generate();
    }
}