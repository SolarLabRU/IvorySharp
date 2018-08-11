namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Набор расширний для <see cref="IInvocationPipeline"/>.
    /// </summary>
    internal static class InvocationPipelineExtensions
    {
        /// <summary>
        /// Возвращает рризнак того, что пайплайн в поврежденном состоянии и продолжение выполнения невозможно.
        /// </summary>
        public static bool IsFaulted(this IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.Faulted &&
                   pipeline.CurrentException != null;
        }

        /// <summary>
        /// Возвращает признак того, что пайплайн в ошибочном состоянии.
        /// </summary>
        public static bool IsExceptional(this IInvocationPipeline pipeline)
        {
            return pipeline.CurrentException != null && (
                       pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                       pipeline.FlowBehavior == FlowBehavior.RethrowException ||
                       pipeline.FlowBehavior == FlowBehavior.Faulted);
        }

        /// <summary>
        /// Возвращает признак того, что пайплайн в состоянии возврата результата.
        /// </summary>
        public static bool IsReturn(this IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.Return;
        }
    }
}