using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта, выполняющего внедрение кода перед и после фактического выполнения метода.
    /// </summary>
    public abstract class MethodBoundaryAspect : OrderableMethodAspect
    {
        /// <summary>
        /// Выполняется перед началом выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnEntry(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется в случае успешного выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnSuccess(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при исключении.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnException(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при завершении метода, независимо от того, произошло исключение или нет.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnExit(IInvocationPipeline pipeline)
        { }
    }
}