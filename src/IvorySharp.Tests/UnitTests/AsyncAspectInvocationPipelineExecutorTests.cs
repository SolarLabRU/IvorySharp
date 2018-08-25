using System;
using System.Threading.Tasks;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Pipeline.Async;
using IvorySharp.Core;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using IvorySharp.Tests.Assets.Invocations;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Базовый класс тестов для проверки <see cref="AsyncInvocationPipelineExecutor"/>.
    /// </summary>
    public partial class AsyncAspectInvocationPipelineExecutorTests
    {
        private readonly BoundaryState[] _normalExecutionStack = {
            new BoundaryState(BoundaryType.Exit),
            new BoundaryState(BoundaryType.Success),
            new BoundaryState(BoundaryType.Entry), 
        };

        private readonly BoundaryState[] _exceptionExecutionStack = {
            new BoundaryState(BoundaryType.Exit),
            new BoundaryState(BoundaryType.Exception),
            new BoundaryState(BoundaryType.Entry),
        };

        
        private static async Task Await(IInvocation invocation)
        {
            await ((Task) invocation.ReturnValue);
        }

        private static async Task<T> Await<T>(IInvocation invocation)
        {
            return await (Task<T>) invocation.ReturnValue;
        }    
        
        private AsyncInvocationPipeline CreatePipeline<TService>(
            TService instace, 
            string methodName,  
            MethodBoundaryAspect[] boundaryAspects,
            params object[] arguments)
        {
            var invocation = new ObservableInvocation(typeof(TService), instace, methodName, arguments);
            var pipeline = new AsyncInvocationPipeline(boundaryAspects, BypassMethodAspect.Instance);

            pipeline.Init(invocation);

            return pipeline;
        }
        
        #region Services

        private interface IService
        {
            Task<int> IdentityAsync(int value);
            Task ThrowArgumentExceptionAsync();
            Task<string> ShouldNotThrow();
        }
        
        public class Service : IService
        {
            public async Task<int> IdentityAsync(int value)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(400));
                return await Task.FromResult(value);
            }

            public async Task ThrowArgumentExceptionAsync()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(400));
                await Task.FromException(new ArgumentException());
            }

            public async Task<string> ShouldNotThrow()
            {
                return await Task.FromException<string>(new ArgumentException());
            }
        }

        #endregion

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
    }
}