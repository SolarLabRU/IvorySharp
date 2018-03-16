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

    /// <summary>
    /// Реализация аспекта для перехвата метода, которая ничего не делает (просто вызывает метод).
    /// </summary>
    internal class NullMethodInterceptionAspect : MethodInterceptionAspect
    {
        /// <summary>
        /// Экземпляр аспекта.
        /// </summary>
        public static NullMethodInterceptionAspect Instance { get; } = new NullMethodInterceptionAspect();
        
        private NullMethodInterceptionAspect() { }
        
        /// <inheritdoc />
        public override void OnInvoke(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}