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
        private readonly AspectInvocationPipelineExecutor _executor;
        
        public AspectInvocationPipelineExecutorTests()
        {
            _executor = AspectInvocationPipelineExecutor.Instance;
        }

        [Fact]
        public void SingleAspect_NormalFlow_AspectBoundariesCalled()
        {
            // Arrange            
            var aspect = new ObservableAspect();
            var pipeline = CreatePipeline<ISingleAspectService>(
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
            
            InvocationAssert.ProceedCalled(pipeline.Invocation);
            Assert.Equal(BoundaryType.Exit, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Success, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }

        
        [Fact]
        public void SingleAspect_ExceptionFlow_AspectBoundariesCalled()
        {
            // Arrange            
            var aspect = new ObservableAspect();
            var pipeline = CreatePipeline<ISingleAspectService>(
                new SingleAspectService(), nameof(ISingleAspectService.ThrowArgumentException), Args.Pack(aspect));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedCalled(pipeline.Invocation);
            Assert.Equal(BoundaryType.Exit, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Exception, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }

        [Fact]
        public void SingleAspect_If_ExceptionOccuring_InBoundary_It_Breakes_Pipeline_And_Throws_Outsite()
        {
            // Arrange            
            var aspect = new ArgumentExceptionThrowAspect(BoundaryType.Entry);
            var pipeline = CreatePipeline<ISingleAspectService>(
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }

        [Fact]
        public void SingleAspect_If_ReturnCalled_InPipeline_Method_ShouldNot_BeCalled()
        {
            // Arrange            
            var aspect = new ReturnDefaultValueAspect(BoundaryType.Entry);
            var pipeline = CreatePipeline<ISingleAspectService>(
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Args.Pack(aspect), Args.Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
           
            InvocationAssert.ProceedNotCalled(pipeline.Invocation);

            Assert.Equal(BoundaryType.Exit, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Success, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }
       
        
        #region Services

        private interface ISingleAspectService
        {
            int Identity(int value);
            void ThrowArgumentException();
        }
        
        private class SingleAspectService : ISingleAspectService
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