namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Набор расширний для <see cref="InvocationPipelineBase"/>.
    /// </summary>
    internal static class InvocationPipelineBaseExtensions
    {
        /// <summary>
        /// Возвращает признак того, что пайплайн в ошибочном состоянии.
        /// </summary>
        public static bool IsExceptional(this InvocationPipelineBase pipeline)
        {
            return pipeline.InternalState == InvocationPipelineState.Exception && 
                   pipeline.CurrentException != null;
        }

        /// <summary>
        /// Возвращает признак того, что пайплайн в состоянии возврата результата.
        /// </summary>
        public static bool IsReturn(this InvocationPipelineBase pipeline)
        {
            return pipeline.InternalState == InvocationPipelineState.Return;
        }
    }
}