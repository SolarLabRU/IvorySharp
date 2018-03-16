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

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public virtual void Initialize() { }
    }
}