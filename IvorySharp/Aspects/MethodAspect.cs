namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый класс для аспектов, применяемых на уровне метода.
    /// </summary>
    public abstract class MethodAspect : AspectAttribute, IMethodAspect
    {
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Порядок атрибута.
        /// </summary>
        public int Order { get; set; }
    }
}