using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Интерфейс аспекта, выполняющего внедрение кода перед и после фактического выполнения метода.
    /// </summary>
    public interface IMethodBoundaryAspect : IMethodAspect
    {
        /// <summary>
        /// Выполняется перед началом выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        void OnEntry(IInvocationPipeline pipeline);
        
        /// <summary>
        /// Выполняется в случае успешного выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        void OnSuccess(IInvocationPipeline pipeline);
        
        /// <summary>
        /// Выполняется при исключении.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        void OnException(IInvocationPipeline pipeline);
        
        /// <summary>
        /// Выполняется при завершении метода, независимо от того, произошло исключение или нет.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        void OnExit(IInvocationPipeline pipeline);
    }
}