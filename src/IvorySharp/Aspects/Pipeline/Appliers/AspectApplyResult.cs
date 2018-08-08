namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Результат применения аспекта.
    /// </summary>
    internal struct AspectApplyResult
    {
        /// <summary>
        /// Аспект, который прервал выполнение пайплайна.
        /// </summary>
        public readonly MethodBoundaryAspect ExecutionBreaker;
        
        /// <summary>
        /// Признак того, что выполнение пайплайна было прервано.
        /// </summary>
        public readonly bool IsExecutionBreaked;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectApplyResult"/>.
        /// </summary>
        /// <param name="executionBreaker">Аспект, который прервал выполнение пайплайна.</param>
        public AspectApplyResult(MethodBoundaryAspect executionBreaker)
        {
            ExecutionBreaker = executionBreaker;
            IsExecutionBreaked = executionBreaker != null;
        }
    }
}