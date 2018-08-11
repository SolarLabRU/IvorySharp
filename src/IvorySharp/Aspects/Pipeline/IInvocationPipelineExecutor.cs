namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент, выполняющий пайплан вызова.
    /// </summary>
    public interface IInvocationPipelineExecutor
    {
        /// <summary>
        /// Выполняет пайплайн.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова.</param>
        void ExecutePipeline(IInvocationPipeline pipeline);
    }
}