namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Вспомогательный класс для работы с потоком выполнения пайплайна.
    /// </summary>
    internal static class InvocationPipelineFlow
    {
        /// <summary>
        /// Возвращает признак того, что пайплайн находится в состоянии ошибки.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова.</param>
        /// <returns>Признак того, что пайплайн находится в состоянии ошибки.</returns>
        internal static bool IsFaulted(IInvocationPipeline pipeline)
        {
            return pipeline.CurrentException != null && (
                       pipeline.FlowBehaviour == FlowBehaviour.ThrowException ||
                       pipeline.FlowBehaviour == FlowBehaviour.RethrowException);
        }
    }
}