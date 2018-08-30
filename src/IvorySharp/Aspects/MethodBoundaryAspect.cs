using IvorySharp.Aspects.Pipeline;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта, выполняющего внедрение кода перед и после фактического выполнения метода.
    /// </summary>
    [PublicAPI]
    public abstract class MethodBoundaryAspect : OrderableMethodAspect
    {
        /// <summary>
        /// Выполняется перед началом выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnEntry([NotNull] IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется в случае успешного выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnSuccess([NotNull] IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при исключении.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnException([NotNull] IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при завершении метода, независимо от того, произошло исключение или нет.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnExit([NotNull] IInvocationPipeline pipeline)
        { }
    }
}