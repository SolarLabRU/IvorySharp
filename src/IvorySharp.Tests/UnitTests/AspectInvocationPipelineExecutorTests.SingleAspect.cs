using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Tests.Assets;
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
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Pack(aspect), Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
            
            Assert.True(IsProceedCalled(pipeline.Invocation));
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
                new SingleAspectService(), nameof(ISingleAspectService.ThrowArgumentException), Pack(aspect));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            Assert.True(IsProceedCalled(pipeline.Invocation));
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
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Pack(aspect), Box(10));
            
            // Act && Assert
            Assert.Throws<ArgumentException>(() => _executor.ExecutePipeline(pipeline));
            
            Assert.False(IsProceedCalled(pipeline.Invocation));
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }

        [Fact]
        public void SingleAspect_If_ReturnCalled_InPipeline_Method_ShouldNot_BeCalled()
        {
            // Arrange            
            var aspect = new ReturnDefaultValueAspect(BoundaryType.Entry);
            var pipeline = CreatePipeline<ISingleAspectService>(
                new SingleAspectService(), nameof(ISingleAspectService.Identity), Pack(aspect), Box(10));
            
            // Act
            _executor.ExecutePipeline(pipeline);
            
            // Assert
            Assert.False(IsProceedCalled(pipeline.Invocation));
            Assert.Equal(BoundaryType.Exit, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Success, aspect.ExecutionStack.Pop().BoundaryType);
            Assert.Equal(BoundaryType.Entry, aspect.ExecutionStack.Pop().BoundaryType);
        }
        
        private AspectInvocationPipeline CreatePipeline<TService>(
            TService instace, 
            string methodName,  
            MethodBoundaryAspect[] boundaryAspects,
            params object[] arguments)
        {
            return new AspectInvocationPipeline(new ObservableInvocation(typeof(TService), instace, methodName, arguments), 
                boundaryAspects, BypassMethodAspect.Instance);
        }

        private static bool IsProceedCalled(IInvocation invocation)
        {
            return invocation is ObservableInvocation oi && oi.IsProceedCalled;
        }

        private static T[] Pack<T>(params T[] args)
        {
            return args;
        }
        
        private static object[] Box<T>(params T[] args)
        {
            return args.Select(a => (object)a).ToArray();
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
        
        #region Aspects 
        
        // TODO : Refactor

        private enum BoundaryType
        {
            Undefined = 0,
            Entry = 1,
            Success = 2,
            Exit = 3,
            Exception = 4,
        }
        
        private class BoundaryState
        {
            public BoundaryState(BoundaryType boundaryType)
            {
                BoundaryType = boundaryType;
            }

            public BoundaryType BoundaryType { get; }
        }
        
        private class ObservableAspect : MethodBoundaryAspect
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
        
        private class ThrowAspect : ObservableAspect
        {
            private readonly Type _exceptionType;
            private readonly BoundaryType _boundaryType;

            public ThrowAspect(Type exceptionType, BoundaryType boundaryType)
            {
                _exceptionType = exceptionType;
                _boundaryType = boundaryType;
            }

            protected override void Entry(IInvocationPipeline pipeline)
            {
                base.Entry(pipeline);
                if (_boundaryType == BoundaryType.Entry)
                {
                    throw CreateException(_exceptionType);
                }
            }

            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                base.OnSuccess(pipeline);
                if (_boundaryType == BoundaryType.Success)
                {
                    throw CreateException(_exceptionType);
                }
            }

            protected override void Exception(IInvocationPipeline pipeline)
            {
                base.Exception(pipeline);
                if (_boundaryType == BoundaryType.Exception)
                {
                    throw CreateException(_exceptionType);
                }
            }

            protected override void Exit(IInvocationPipeline pipeline)
            {
                base.Exit(pipeline);
                if (_boundaryType == BoundaryType.Exit)
                {
                    throw CreateException(_exceptionType);
                }      
            }

            protected static Exception CreateException(Type exceptionType)
            {
                return (System.Exception) Activator.CreateInstance(exceptionType);                       
            }
        }

        private class ArgumentExceptionThrowAspect : ThrowAspect
        {
            public ArgumentExceptionThrowAspect(BoundaryType boundaryType) 
                : base(typeof(ArgumentException), boundaryType)
            {
            }
        }
        
        private class ReturnDefaultValueAspect : ObservableAspect
        {
            private readonly BoundaryType _boundaryType;

            public ReturnDefaultValueAspect(BoundaryType boundaryType)
            {
                _boundaryType = boundaryType;
            }
            
            protected override void Entry(IInvocationPipeline pipeline)
            {
                base.Entry(pipeline);
                if (_boundaryType == BoundaryType.Entry)
                {
                    pipeline.Return();
                }
            }

            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                base.OnSuccess(pipeline);
                if (_boundaryType == BoundaryType.Success)
                {
                    pipeline.Return();
                }
            }

            protected override void Exception(IInvocationPipeline pipeline)
            {
                base.Exception(pipeline);
                if (_boundaryType == BoundaryType.Exception)
                {
                    pipeline.Return();
                }
            }

            protected override void Exit(IInvocationPipeline pipeline)
            {
                base.Exit(pipeline);
                if (_boundaryType == BoundaryType.Exit)
                {
                    pipeline.Return();
                }      
            }
        }

        #endregion
    }
}