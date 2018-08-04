using IvorySharp.Core;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Реализация аспекта для перехвата метода, которая ничего не делает (просто вызывает метод).
    /// </summary>
    internal class BypassMethodAspect : MethodInterceptionAspect
    {
        /// <summary>
        /// Экземпляр аспекта.
        /// </summary>
        public static BypassMethodAspect Instance { get; } = new BypassMethodAspect();
        
        private BypassMethodAspect() { }
        
        /// <inheritdoc />
        public override void OnInvoke(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}