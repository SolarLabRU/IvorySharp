namespace IvoryProxy.Core
{
    /// <summary>
    /// Генератор динамических прокси объектов.
    /// </summary>
    public interface IProxyGenerator
    {
        /// <summary>
        /// Создает прокси для интерфейса типа <typeparamref name="T"/>.
        /// </summary>
        /// <param name="source">Исходный объект.</param>
        /// <typeparam name="T">Тип объекта для проксирования. Обязательно интерфейс.</typeparam>
        /// <returns>Прокси объекта <paramref name="source"/>.</returns>
        IIvoryProxy<T> CreateInterfaceProxy<T>(T source) where T : class;
    }
}