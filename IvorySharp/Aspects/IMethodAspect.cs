namespace IvorySharp.Aspects
{
    /// <summary>
    /// Интерфейс аспекта, применяемого на метод.
    /// </summary>
    public interface IMethodAspect
    {
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// Порядок атрибута. Меньшее значение порядка значит более высокий приоритет аспекта.
        /// То есть, аспект с <see cref="Order"/> = 0 будет выполнен раньше,
        /// чем аспект с <see cref="Order"/> = 1.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Выполняет инициализацию аспекта.
        /// </summary>
        void Initialize();
    }
}