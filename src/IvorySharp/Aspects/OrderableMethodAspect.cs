using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Аспект, который можно упорядочить.
    /// </summary>
    [PublicAPI]
    public abstract class OrderableMethodAspect : MethodAspect
    {
        /// <summary>
        /// Порядок атрибута. Меньшее значение порядка значит более высокий приоритет аспекта.
        /// То есть, аспект с <see cref="Order"/> = 0 будет выполнен раньше,
        /// чем аспект с <see cref="Order"/> = 1.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Внутренний порядок аспекта (Order + Index).
        /// </summary>
        internal int InternalOrder { get; set; }
    }
}