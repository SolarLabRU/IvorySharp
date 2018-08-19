namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Действие, применяемое к пайплайну метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal abstract class InvocationState<TPipeline> where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Выполняет переход выполнения метожа в следующее состояние.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения метода.</param>
        /// <returns>Следующеее состояние пайплайна. Если результат null, значит пайплайн завершен.</returns>
        internal abstract InvocationState<TPipeline> MakeTransition(TPipeline pipeline);
    }
}