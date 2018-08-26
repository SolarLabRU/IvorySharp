namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Действие, применяемое к пайплайну метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal interface IInvocationState<in TPipeline> where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Выполняет переход выполнения метожа в следующее состояние.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения метода.</param>
        /// <returns>Следующеее состояние пайплайна. Если результат null, значит пайплайн завершен.</returns>
        IInvocationState<TPipeline> MakeTransition(TPipeline pipeline);
    }
}