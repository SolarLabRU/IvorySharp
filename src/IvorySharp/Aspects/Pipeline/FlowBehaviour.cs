namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Возможные состояния выполнения потока программы.
    /// </summary>
    public enum FlowBehaviour
    {
        /// <summary>
        /// Значение по умолчанию.
        /// Для хендлеров
        /// <see cref="MethodBoundaryAspect.OnEntry(IInvocationPipeline)"/>,
        /// <see cref="MethodBoundaryAspect.OnSuccess(IInvocationPipeline)"/>,
        /// <see cref="MethodBoundaryAspect.OnExit(IInvocationPipeline)"/>
        /// равно выполняется продолжение.
        /// Для хендлера <see cref="MethodBoundaryAspect.OnException(IInvocationPipeline)"/>
        /// эквивалетно значению <see cref="FlowBehaviour.RethrowException"/>.
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// Прекращение выполнения пайплайна с возвратом текущего результата из контекста.
        /// </summary>
        Return = 1,
        
        /// <summary>
        /// Прокидывает исключение во вне, прекращая выполнение пайплайна.
        /// </summary>
        ThrowException = 2,
        
        /// <summary>
        /// Прокидывает исключение дальше по пайплану с сохранением всего стека.
        /// </summary>
        RethrowException = 3
    }
}