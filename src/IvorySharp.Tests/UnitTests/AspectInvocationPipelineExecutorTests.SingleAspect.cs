using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Tests.Asserts;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using IvorySharp.Tests.Utility;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="AspectInvocationPipelineExecutor"/> для одного аспекта.
    /// </summary>
    public partial class AspectInvocationPipelineExecutorTests
    {
        [Fact]
        public void SingleAspect_NormalFlow_AspectBoundariesCalled()
        {
            // Arrange            
            var aspect = new ObservableAspect();
            var pipeline = CreateObservablePipeline<IService>(
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
            var pipeline = CreateObservablePipeline<IService>(
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
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }
        
        [Fact]
        public void SingleAspect_If_HandledException_Occuring_InBoundary_OnEntry_OnExit_Called()
        {
            // Arrange            
            var aspect = new ThrowAspect(typeof(ArgumentException), BoundaryType.Entry);
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(new[]
            {
                new BoundaryState(BoundaryType.Exit),
                new BoundaryState(BoundaryType.Entry), 
            }, aspect.ExecutionStack);
        }

        [Fact]
        public void SingleAspect_If_ReturnCalled_InPipeline_Method_ShouldNot_BeCalled()
        {
            // Arrange            
            var aspect = new ReturnDefaultValueAspect(BoundaryType.Entry);
            var pipeline = CreateObservablePipeline<IService>(
                new Service(), nameof(IService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
           
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(_normalExecutionStack, aspect.ExecutionStack);
        }
    }
}