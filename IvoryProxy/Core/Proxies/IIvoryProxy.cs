namespace IvoryProxy.Core.Proxies
{
    /// <summary>
    /// Интерфейс динамическго прокси.
    /// </summary>
    /// <typeparam name="T">Тип объекта для проксирования.</typeparam>
    public interface IIvoryProxy<out T>
        where T : class
    {
        /// <summary>
        /// Объект без изменений.
        /// </summary>
        T Decorated { get; }
        
        /// <summary>
        /// Прокси объекта.
        /// </summary>
        T TransparentProxy { get; }
    }
}