using IvorySharp.Core;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта для перехвата вызова метода. 
    /// </summary>
    public abstract class MethodInterceptionAspect : MethodAspect, IMethodInterceptionAspect
    {
        /// <inheritdoc />
        public abstract void OnInvoke(IInvocation invocation);
    }
}