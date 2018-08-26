using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние вызова исходного метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна,</typeparam>
    internal sealed class MethodCallState<TPipeline> : InvocationState<TPipeline>
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Аспект перехвата вызова метода.
        /// </summary>
        public MethodInterceptionAspect InterceptionAspect { get; }

        /// <summary>
        /// Аспекты, прикрепленные к методу.
        /// </summary>
        public IEnumerable<MethodBoundaryAspect> BoundaryAspects { get; }

        public MethodCallState(
            MethodInterceptionAspect interceptionAspect,
            IEnumerable<MethodBoundaryAspect> boundaryAspects)
        {
            InterceptionAspect = interceptionAspect;
            BoundaryAspects = boundaryAspects;
        }

        /// <inheritdoc />
        internal override InvocationState<TPipeline> MakeTransition(TPipeline pipeline)
        {
            try
            {
                // Вызов метода должен происходить только при нормальном состоянии пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                    InterceptionAspect.OnInvoke(pipeline.Invocation);

                // OnEntry -> MethodCall [success] -> OnSuccess -> OnExit
                return new SuccessState<TPipeline>(BoundaryAspects);
            }
            catch (Exception e)
            {
                var innerException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);

                //  OnEntry -> MethodCall [exception] -> OnException -> OnExit
                pipeline.Continue(innerException);

                return new CatchState<TPipeline>(BoundaryAspects);
            }
        }
    }
}