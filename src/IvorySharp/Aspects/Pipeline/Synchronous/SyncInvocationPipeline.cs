using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline.Synchronous
{
    /// <summary>
    /// Базовая модель синхронного пайплайна вызова метода.
    /// </summary>
    internal abstract class SyncInvocationPipeline : InvocationPipelineBase
    {
        /// <inheritdoc />
        public override object CurrentReturnValue
        {
            get => Invocation.ReturnValue;
            set => Invocation.ReturnValue = value;
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="SyncInvocationPipeline"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        protected SyncInvocationPipeline(IInvocation invocation) : base(invocation)
        {
        }
    }
}