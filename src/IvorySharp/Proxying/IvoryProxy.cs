using System.ComponentModel;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Базовый класс прокси.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class IvoryProxy
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="IvoryProxy"/>.
        /// </summary>
        protected internal IvoryProxy() { }

        /// <summary>
        /// При каждом вызове метода прокси, вызывается данный метод для делегирования вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова проксированного метода.</param>
        /// <returns>Результат выполнения метода.</returns>
        protected internal abstract object Invoke(MethodInvocation invocation);
    }
}