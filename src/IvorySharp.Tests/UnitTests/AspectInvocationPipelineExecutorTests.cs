using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using IvorySharp.Tests.Assets.Invocations;

namespace IvorySharp.Tests.UnitTests
{
    public partial class AspectInvocationPipelineExecutorTests
    {
        private readonly BoundaryState[] _normalExecutionStack = new BoundaryState[]
        {
            new BoundaryState(BoundaryType.Exit),
            new BoundaryState(BoundaryType.Success),
            new BoundaryState(BoundaryType.Entry), 
        };

        private readonly BoundaryState[] _exceptionExecutionStack = new[]
        {
            new BoundaryState(BoundaryType.Exit),
            new BoundaryState(BoundaryType.Exception),
            new BoundaryState(BoundaryType.Entry),
        };
        
        private readonly InvocationPipelineExecutor _executor;
        
        public AspectInvocationPipelineExecutorTests()
        {
            _executor = InvocationPipelineExecutor.Instance;
        }
        
        private InvocationPipeline CreatePipeline<TService>(
            TService instace, 
            string methodName,  
            MethodBoundaryAspect[] boundaryAspects,
            params object[] arguments)
        {
            return new InvocationPipeline(
                new ObservableInvocation(typeof(TService), instace, methodName, arguments), 
                boundaryAspects, BypassMethodAspect.Instance);
        }
        
        #region Aspects

        private class IncrementReturnValueOnSuccess : ObservableAspect
        {
            protected override void Success(IInvocationPipeline pipeline)
            {
                var result = (int)pipeline.CurrentReturnValue;
                pipeline.ReturnValue(result + 1);
            }
        }

        #endregion
            
        #region Services

        private interface IService
        {
            int Identity(int value);
            void ThrowArgumentException();
            string ShouldNotThrow();
        }
        
        private class Service : IService
        {
            public int Identity(int value)
            {
                return value;
            }

            public void ThrowArgumentException()
            {
                throw new ArgumentException();
            }

            public string ShouldNotThrow()
            {
                throw new ArgumentException();
            }
        }

        #endregion
    }
}