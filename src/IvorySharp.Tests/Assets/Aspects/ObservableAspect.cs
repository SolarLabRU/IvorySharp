using System.Collections.Generic;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ObservableAspect : MethodBoundaryAspect
    {
        public Stack<BoundaryState> ExecutionStack { get; }
            
        public ObservableAspect()
        {
            ExecutionStack = new Stack<BoundaryState>();
        }
            
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            ExecutionStack.Push(new BoundaryState(BoundaryType.Entry));
            Entry(pipeline);
        }

        protected virtual void Entry(IInvocationPipeline pipeline) { }
            
        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            ExecutionStack.Push(new BoundaryState(BoundaryType.Success));
            Success(pipeline);
        }

        protected virtual void Success(IInvocationPipeline pipeline) { }

        public override void OnException(IInvocationPipeline pipeline)
        {
            ExecutionStack.Push(new BoundaryState(BoundaryType.Exception));
            Exception(pipeline);
        }

        protected virtual void Exception(IInvocationPipeline pipeline) { }

        public override void OnExit(IInvocationPipeline pipeline)
        {
            ExecutionStack.Push(new BoundaryState(BoundaryType.Exit));
            Exit(pipeline);
        }

        protected virtual void Exit(IInvocationPipeline pipeline) { }
    }
}