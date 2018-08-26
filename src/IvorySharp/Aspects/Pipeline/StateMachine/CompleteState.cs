using System.Runtime.CompilerServices;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние завершения пайплайна.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class CompleteState<TPipeline> : IInvocationState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Экземпляр <see cref="CompleteState{TPipeline}"/>.
        /// </summary>
        public static readonly CompleteState<TPipeline> Instance = new CompleteState<TPipeline>();
        
        private CompleteState() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IInvocationState<TPipeline> MakeTransition(TPipeline pipeline)
        {
            if (pipeline.IsExceptional())
                pipeline.CurrentException.Throw();

            return null;
        }
    }
}