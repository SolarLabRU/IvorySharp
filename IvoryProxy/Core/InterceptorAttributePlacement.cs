namespace IvoryProxy.Core
{
    /// <summary>
    /// Место размещения атрибута.
    /// </summary>
    public enum InterceptorAttributePlacement
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Атрибут размещен на методе.
        /// </summary>
        Method = 1,
        
        /// <summary>
        /// Атрибут размещен на интерфейсе.
        /// </summary>
        Interface = 2
    }
}