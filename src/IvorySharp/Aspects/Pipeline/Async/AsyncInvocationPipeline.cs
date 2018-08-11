using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline.Async
{
    /// <summary>
    /// Базовая модель пайлпайна выполнения асинхронного метода.
    /// </summary>
    internal abstract class AsyncInvocationPipeline : InvocationPipelineBase
    {
        /// <inheritdoc />
        public override object CurrentReturnValue { get; set; }

        protected AsyncInvocationPipeline(IInvocation invocation)
            : base(invocation)
        {
        }
    }
}