using System;
using System.Collections.Generic;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Assets;

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
        
        private readonly AspectInvocationPipelineExecutor _executor;
        
        public AspectInvocationPipelineExecutorTests()
        {
            _executor = AspectInvocationPipelineExecutor.Instance;
        }
        
        private AspectInvocationPipeline CreateObservablePipeline<TService>(
            TService instace, 
            string methodName,  
            MethodBoundaryAspect[] boundaryAspects,
            params object[] arguments)
        {
            return new AspectInvocationPipeline(
                new ObservableInvocation(typeof(TService), instace, methodName, arguments), 
                boundaryAspects, BypassMethodAspect.Instance);
        }
        
        #region Services

        private interface IService
        {
            int Identity(int value);
            void ThrowArgumentException();
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
        }

        #endregion
    }
}