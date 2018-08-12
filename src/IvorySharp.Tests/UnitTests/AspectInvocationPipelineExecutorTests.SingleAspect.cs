using System;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Tests.Asserts;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using IvorySharp.Tests.Utility;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="SyncAspectInvocationPipelineExecutor"/> для одного аспекта.
    /// </summary>
    public partial class AspectInvocationPipelineExecutorTests
    {
        [Fact]
        public void SingleAspect_NormalFlow_AspectBoundariesCalled()
        {
            // Arrange            
            var aspect = new ObservableAspect();
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert       
            InvocationAssert.ProceedCalled(pipeline.Invocation);
            Assert.Equal(_normalExecutionStack, aspect.ExecutionStack);
        }

        
        [Fact]
        public void SingleAspect_ExceptionFlow_AspectBoundariesCalled()
        {
            // Arrange            
            var aspect = new ObservableAspect();
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.ThrowArgumentException), Args.Pack(aspect));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedCalled(pipeline.Invocation);
            Assert.Equal(_exceptionExecutionStack, aspect.ExecutionStack);
        }

        [Fact]
        public void SingleAspect_If_UnhandledException_Occuring_InBoundary_It_Breakes_Pipeline_And_Throws_Outsite()
        {
            // Arrange            
            var aspect = new ThrowAspect(typeof(ArgumentException), BoundaryType.Entry, throwAsUnhandled: true);
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }
        
        [Fact]
        public void SingleAspect_If_HandledException_Occuring_InBoundary_OnEntry_OtherAspectsShouldNotBeCalled()
        {
            // Arrange            
            var aspect = new ThrowAspect(typeof(ArgumentException), BoundaryType.Entry);
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(new[]
            {
                new BoundaryState(BoundaryType.Entry), 
            }, aspect.ExecutionStack);
        }

        [Fact]
        public void SingleAspect_If_ReturnCalled_InPipeline_Method_ShouldNot_BeCalled()
        {
            // Arrange            
            var aspect = new ReturnDefaultValueAspect(BoundaryType.Entry);
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
           
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            
            Assert.Equal(new[]
            {
                new BoundaryState(BoundaryType.Entry)
            }, aspect.ExecutionStack);
        }
        
        [Fact]
        public void SingleAspect_CallReturn_AfterMethodExecution_ShouldChangeResult()
        {
            // Arrange            
            var aspect = new IncrementReturnValueOnSuccess();
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
            Assert.Equal(11, pipeline.Invocation.ReturnValue);
            Assert.Equal(_normalExecutionStack, aspect.ExecutionStack);
            Assert.Equal(11, pipeline.CurrentReturnValue);
            
            InvocationAssert.ProceedCalled(pipeline.Invocation);  
        }
        
        [Fact]
        public void SingleAspect_CallThrow_AfterMethodExecution_ShouldThrow()
        {
            // Arrange            
            var aspect = new ThrowAspect(typeof(ArgumentException), BoundaryType.Success, throwAsUnhandled: false);
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            Assert.Throws<ArgumentException>(() =>  _executor.ExecutePipeline(pipeline));

            // Assert
            Assert.Equal(_normalExecutionStack, aspect.ExecutionStack);
            
            Assert.IsType<ArgumentException>(pipeline.CurrentException);          
            InvocationAssert.ProceedCalled(pipeline.Invocation);
        }

        [Fact]
        public void SingleAspect_SwallowException_SetResult_ShoultNotThrow()
        {
            // Arrange
            var aspect = new ReturnValueAspect(BoundaryType.Exception, "hello world");
            var pipeline = CreatePipeline<IService>(
                new Service(), nameof(IService.ShouldNotThrow), Args.Pack(aspect));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
            
            Assert.Equal(new[]
            {
                new BoundaryState(BoundaryType.Exception),
                new BoundaryState(BoundaryType.Entry), 
            }, aspect.ExecutionStack);
            
            Assert.Equal("hello world", pipeline.Invocation.ReturnValue);
            Assert.Equal("hello world", pipeline.CurrentReturnValue);
            InvocationAssert.ProceedCalled(pipeline.Invocation);
        }
    }
}