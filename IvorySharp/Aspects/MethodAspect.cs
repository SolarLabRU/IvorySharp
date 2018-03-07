using IvorySharp.Aspects.Configuration;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый класс для аспектов, применяемых на уровне метода.
    /// </summary>
    public abstract class MethodAspect : AspectAttribute, IMethodAspect
    {
        /// <summary>
        /// Признак наличия зависимостей.
        /// </summary>
        internal bool HasDependencies { get; set; }
        
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Порядок атрибута. Меньшее значение порядка значит более высокий приоритет аспекта.
        /// То есть, аспект с <see cref="Order"/> = 0 будет выполнен раньше,
        /// чем аспект с <see cref="Order"/> = 1.
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Выполняет инициализацию аспекта. Должен переопределяться в наследниках.
        /// </summary>
        /// <param name="settings">Модель настроек.</param>
        public virtual void Initialize(IAspectsWeavingSettings settings) { }
    }
}