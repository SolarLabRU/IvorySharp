namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Машина состояний, реализующая переход состояния вызова метода между аспектами.
    /// Этот компонент является координатором выполнения метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class InvocationStateMachine<TPipeline> where TPipeline : InvocationPipelineBase
    {
        private readonly TPipeline _pipeline;

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationStateMachine{TPipeline}"/>.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения метода.</param>
        public InvocationStateMachine(TPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        /// <summary>
        /// Инициирует выполнение стейт-машины обработки пайплайна вызова метода.
        /// </summary>
        /// <param name="initialState">Начальное состояние стейт-машины.</param>
        public void Execute(InvocationState<TPipeline> initialState)
        {
            var nextState = initialState.MakeTransition(_pipeline);
            while (nextState != null)
            {
                nextState = nextState.MakeTransition(_pipeline);
            }
        }
    }
}