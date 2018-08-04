using System.Collections.Generic;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта, выполняющего внедрение кода перед и после фактического выполнения метода.
    /// </summary>
    public abstract class MethodBoundaryAspect : MethodAspect
    {
        /// <summary>
        /// Порядок атрибута. Меньшее значение порядка значит более высокий приоритет аспекта.
        /// То есть, аспект с <see cref="Order"/> = 0 будет выполнен раньше,
        /// чем аспект с <see cref="Order"/> = 1.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Выполняется перед началом выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnEntry(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется в случае успешного выполнения метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnSuccess(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при исключении.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnException(IInvocationPipeline pipeline)
        { }

        /// <summary>
        /// Выполняется при завершении метода, независимо от того, произошло исключение или нет.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        public virtual void OnExit(IInvocationPipeline pipeline)
        { }
    }
}