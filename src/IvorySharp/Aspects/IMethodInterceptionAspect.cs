using IvorySharp.Core;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Интерфейс аспекта для перехвата выполнения метода.
    /// </summary>
    public interface IMethodInterceptionAspect
    {
        /// <summary>
        /// Выполняет перехват вызова метода.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        void OnInvoke(IInvocation invocation);
    }
}