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
        T Proxy { get; }

        /// <summary>
        /// Выполняет вызов метода в соответствии с моделью <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Результат выполнения метода.</returns>
        void Proceed(IMethodInvocation invocation);
    }
}