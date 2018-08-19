using System.Threading.Tasks;

namespace IvorySharp.Aspects.Pipeline.Async.StateMachine
{
    /// <summary>
    /// Состояние выполнения пайплайна вызова асинхронного метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal abstract class AsyncInvocationState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Выполняет переход из текущего состояния пайплайна в следующее.
        /// Вызывается для методов, возвращающих <see cref="Task{TResult}"/>.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова.</param>
        /// <typeparam name="TResult">Тип возвращаемого результата метода.</typeparam>
        /// <returns>Следующее состояние пайплайна, если null - пайплайн завершен.</returns>
        public abstract Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync<TResult>(TPipeline pipeline);

        /// <summary>
        /// Выполняет переход из текущего состояния пайплайна в следующее.
        /// Вызывается для методов, возвращающих <see cref="Task"/>.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова.</param>
        /// <returns>Следующее состояние пайплайна, если null - пайплайн завершен.</returns>
        public abstract Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync(TPipeline pipeline);
    }
}