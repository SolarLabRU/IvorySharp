using System.ComponentModel;
using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Возможные состояния выполнения потока программы.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public enum FlowBehavior
    {
        /// <summary>
        /// Значение по умолчанию - пайплайн выполяется в нормальном режиме.
        /// Ожидаемая последовательность вызова:
        ///     <see cref="MethodBoundaryAspect.OnEntry"/>
        ///     <see cref="IInvocation.Proceed()"/> (вызов метода)
        ///     <see cref="MethodBoundaryAspect.OnSuccess"/>
        ///     <see cref="MethodBoundaryAspect.OnExit"/>
        /// </summary>
        Continue = 0,
        
        /// <summary>
        /// Прекращает выполнение пайплайна с возвратом результата.
        /// У всех обработчиков, успевших выполниться до текущего - вызываются хендлеры
        ///     <see cref="MethodBoundaryAspect.OnSuccess"/>
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        ///
        /// У обработчиков с более низким приоритетом
        /// (большим значением <see cref="OrderableMethodAspect.Order"/>)
        /// никакие из точек прикрепления внутри метода выполнены не будут.
        /// </summary>
        Return = 1,
        
        /// <summary>
        /// Прекращает выполнение пайплайна с исключением.
        /// У всех обработчиков, успевших выполниться до текущего - вызывается хендлер
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        /// 
        /// У обработчиков с более низким приоритетом
        /// (большим значением <see cref="OrderableMethodAspect.Order"/>)
        /// никакие из точек прикрепления внутри метода выполнены не будут.
        /// </summary>
        ThrowException = 2,
        
        /// <summary>
        /// Прекращает выполнение пайплайна с перебрасыванием исключения.
        /// У всех обработчиков, успевших выполниться до текущего - вызываются хендлеры
        ///     <see cref="MethodBoundaryAspect.OnException"/>
        ///     <see cref="MethodBoundaryAspect.OnExit"/>.
        /// 
        /// У обработчиков с более низким приоритетом
        /// (большим значением <see cref="OrderableMethodAspect.Order"/>)
        /// никакие из точек прикрепления внутри метода выполнены не будут.
        /// </summary>
        RethrowException = 3,
        
        /// <summary>
        /// Обновление возвращаемого значения без прекращения выполнения пайплайна.
        /// </summary>
        UpdateReturnValue = 4,
    }
}