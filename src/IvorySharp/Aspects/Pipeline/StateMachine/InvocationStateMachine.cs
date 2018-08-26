using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Машина состояний, реализующая переход состояния вызова метода между аспектами.
    /// Этот компонент является координатором выполнения метода.
    /// </summary>
    internal static class InvocationStateMachine
    {
        /// <summary>
        /// Инициирует выполнение стейт-машины обработки пайплайна вызова метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова.</param>
        /// <param name="initialState">Начальное состояние стейт-машины.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Execute<TPipeline>(
            [NotNull] TPipeline pipeline,
            [NotNull] IInvocationState<TPipeline> initialState)
                where TPipeline : InvocationPipelineBase
        {
            var nextState = initialState.MakeTransition(pipeline);
            while (nextState != null)
            {
                nextState = nextState.MakeTransition(pipeline);
            }
        }
    }
}