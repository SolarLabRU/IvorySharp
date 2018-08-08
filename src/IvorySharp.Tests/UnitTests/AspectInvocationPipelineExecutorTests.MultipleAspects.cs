using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Asserts;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using IvorySharp.Tests.Utility;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    public partial class AspectInvocationPipelineExecutorTests
    { 
        [Fact]
        public void MultipleAspects_OnEntry_ReturnResult_OnSuccess_Called_OnTriggered_Aspect_But_Not_On_OuterAspects()
        {
            // Arrange
            var afterBreaker1 = new ObservableAspect { InternalOrder = 3};
            var afterBreaker2 = new ObservableAspect { InternalOrder = 2};
            var breaker = new ReturnDefaultValueAspect(BoundaryType.Entry) { InternalOrder = 1};
            var beforeBreaker = new ObservableAspect{ InternalOrder = 0};
            
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), 
                Args.Pack(beforeBreaker, breaker, afterBreaker1, afterBreaker2), 
                Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert

            // Метод не вызвался, т.к. 2й аспект вернул результат
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            // Первый аспект прошел весь нормальный флоу
            Assert.Equal(_normalExecutionStack, beforeBreaker.ExecutionStack);
            // Второй вернул результат и тоже успешно выполнился
            Assert.Equal(_normalExecutionStack, breaker.ExecutionStack);
            // Остальные не выполнились
            Assert.Empty(afterBreaker1.ExecutionStack);
            Assert.Empty(afterBreaker2.ExecutionStack);
        }

        [Fact]
        public void MultipleAspects_OnException_RethrowException_Handles_InInnerAspect_ButNot_In_Outer()
        {
            // Arrange
            var afterBreaker = new ObservableAspect { InternalOrder = 2};
            var breaker = new RethrowAspect(typeof(ApplicationException)) { InternalOrder = 1 };
            var beforeBreaker = new RethrowAspect(typeof(ArgumentException)) { InternalOrder = 0};
            
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.ThrowArgumentException), 
                Args.Pack<MethodBoundaryAspect>(beforeBreaker, breaker, afterBreaker));
            
            // Assert
            Assert.Throws<ApplicationException>(() => _executor.ExecutePipeline(pipeline));        
            Assert.Equal(_exceptionExecutionStack, breaker.ExecutionStack);     
            Assert.Equal(_exceptionExecutionStack, beforeBreaker.ExecutionStack);
        }

        [Fact]
        public void Multiple_OnException_ThrowException_In_OnEntry_Handled_InnerAspects_OnEntry_OnExit_Called()
        {
            // Arrange
            var afterBreaker = new ObservableAspect { InternalOrder = 2 };
            var breaker = new ThrowAspect(typeof(ApplicationException), BoundaryType.Entry) { InternalOrder = 1 };
            var beforeBreaker = new ObservableAspect() { InternalOrder = 0};
            
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), 
                Args.Pack<MethodBoundaryAspect>(beforeBreaker, breaker, afterBreaker),
                Args.Box(10));
            
            // Assert
            Assert.Throws<ApplicationException>(() => _executor.ExecutePipeline(pipeline)); 
            Assert.Empty(afterBreaker.ExecutionStack);
            Assert.Equal(new []
            {
                new BoundaryState(BoundaryType.Exit),
                new BoundaryState(BoundaryType.Entry), 
            }, beforeBreaker.ExecutionStack);
            
            Assert.Equal(new []
            {
                new BoundaryState(BoundaryType.Exit),
                new BoundaryState(BoundaryType.Entry), 
            }, breaker.ExecutionStack);
        }
        
        [Fact]
        public void Multiple_OnException_ThrowException_In_OnEntry_Unhandled_BreakesPipeline()
        {
            // Arrange
            var afterBreaker = new ObservableAspect { InternalOrder = 2 };
            var breaker = new ThrowAspect(typeof(ApplicationException), BoundaryType.Entry, throwAsUnhandled: true)
            {
                InternalOrder = 1
            };
            
            var beforeBreaker = new ObservableAspect() { InternalOrder = 0};
            
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), 
                Args.Pack<MethodBoundaryAspect>(beforeBreaker, breaker, afterBreaker),
                Args.Box(10));
            
            // Assert
            Assert.Throws<ApplicationException>(() => _executor.ExecutePipeline(pipeline)); 
            Assert.Empty(afterBreaker.ExecutionStack);
            Assert.Equal(new []
            {
                new BoundaryState(BoundaryType.Entry)
            }, beforeBreaker.ExecutionStack);
            
            Assert.Equal(new []
            {
                new BoundaryState(BoundaryType.Entry) 
            }, breaker.ExecutionStack);
        }
        
        private class RethrowAspect : ObservableAspect
        {
            private readonly Type _exceptionType;

            public RethrowAspect(Type exceptionType)
            {
                _exceptionType = exceptionType;
            }

            protected override void Exception(IInvocationPipeline pipeline)
            {
                pipeline.RethrowException(CreateException(_exceptionType, pipeline.CurrentException));
            }

            protected static Exception CreateException(Type exceptionType, Exception inner)
            {
                return (Exception) Activator.CreateInstance(exceptionType, string.Empty, inner);                       
            }
        }
    }
}