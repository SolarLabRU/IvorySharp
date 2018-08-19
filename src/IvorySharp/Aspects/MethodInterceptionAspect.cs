using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта для перехвата вызова метода. 
    /// </summary>
    [PublicAPI]
    public abstract class MethodInterceptionAspect : MethodAspect
    {
        /// <summary>
        /// Выполняет перехват вызова метода.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        public abstract void OnInvoke([NotNull] IInvocation invocation);
    }
}