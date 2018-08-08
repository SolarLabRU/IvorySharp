using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

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
        /// <param name="targetMethod">Целевой метод.</param>
        /// <param name="args">Параметры вызова метода.</param>
        /// <returns>Результат выполнения метода.</returns>
        protected internal abstract object Invoke([NotNull] MethodInfo targetMethod, [NotNull] object[] args);
    }
}