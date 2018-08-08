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
    }
}