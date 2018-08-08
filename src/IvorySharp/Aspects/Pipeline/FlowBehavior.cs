using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Возможные состояния выполнения потока программы.
    /// </summary>
    [PublicAPI]
    public enum FlowBehavior
    {
        /// <summary>
        /// Значение по умолчанию.
        /// Для хендлеров
        /// <see cref="MethodBoundaryAspect.OnEntry(IInvocationPipeline)"/>,
        /// <see cref="MethodBoundaryAspect.OnSuccess(IInvocationPipeline)"/>,
        /// <see cref="MethodBoundaryAspect.OnExit(IInvocationPipeline)"/>
        /// равно выполняется продолжение.
        /// Для хендлера <see cref="MethodBoundaryAspect.OnException(IInvocationPipeline)"/>
        /// эквивалетно значению <see cref="FlowBehavior.RethrowException"/>.
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// Прекращение выполнения пайплайна с возвратом результата.
        /// У всех обработчико до и включая текущий, которые успели
        /// выполниться - вызываются хендлеры
        ///     <see cref="MethodBoundaryAspect.OnSuccess"/>
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        /// </summary>
        Return = 1,
        
        /// <summary>
        /// Прекращает выполнение пайплайна с исключением.
        /// У всех обработчиков до и включая текущий, которые успели
        /// выполниться - вызывается хендлер
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        /// </summary>
        ThrowException = 2,
        
        /// <summary>
        /// Прекращает выполнение пайплайна с исключением.
        /// У всех обработчиков до и включая текущий, которые успели
        /// выполниться - вызываются хендлеры
        ///     <see cref="MethodBoundaryAspect.OnException"/>
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        /// </summary>
        RethrowException = 3,
        
        /// <summary>
        /// Пайплайн был внезапно прекращен.
        /// Обычно это значит что при выполнении какого-то хендлера
        /// произошло необработнное исключение.
        /// Никакие внешние и текущие обработчики выполнены не будут.
        /// Возникшее исключение попадет в вызывающий код "как есть".
        /// </summary>
        Faulted = 4
    }
}