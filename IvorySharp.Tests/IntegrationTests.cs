using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Weaving;
using Xunit;

namespace IvorySharp.Tests
{
    internal class DummyConfigurations : IWeavingAspectsConfiguration
    {
        public Type ExplicitWeaingAttributeType { get; } = null;
    }
    
    public class IntegrationTests
    {
        private AspectWeaver _aspectWeaver;
        
        public IntegrationTests()
        {
            _aspectWeaver = new AspectWeaver(new DummyConfigurations());
            BaseObservableAspect.ResetFlags();
        }
        
        [Fact]
        public void SingleMethodBoundaryAspect_NoExceptionFlow()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            proxy.BypassAspectSingle();
            
            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.True(BaseObservableAspect.IsOnSuccessCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
        }

        [Fact]
        public void SingleMethodBoundaryAspect_ExceptionHandler_NotHandle_Exeption_Rethrows()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var exception = Assert.Throws<ArgumentException>(() => proxy.ThrowsExceptionNotHandled());
            
            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.False(BaseObservableAspect.IsOnSuccessCalled);
            Assert.True(BaseObservableAspect.IsOnExceptionCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
        }

        [Fact]
        public void SingleMethodBoundaryAspect_ExceptionHandler_ThrowsNewException()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => proxy.ThrowsNewException());
            
            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.False(BaseObservableAspect.IsOnSuccessCalled);
            Assert.True(BaseObservableAspect.IsOnExceptionCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
        }

        [Fact]
        public void SingleMethodBoundaryAspect_ExceptionHandler_SwallowException()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var result = proxy.SwallowExceptionWithVal();
            
            // Assert
            Assert.Equal("swallowed", result);
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.False(BaseObservableAspect.IsOnSuccessCalled);
            Assert.True(BaseObservableAspect.IsOnExceptionCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
        }

        [Fact]
        public void MultipleMethodBoundaryAspects_OnEntry_Exception_NotHandledByHandler()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var result = Assert.Throws<AccessViolationException>(() => proxy.ThrowsOnEntry());

            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
            Assert.False(BaseObservableAspect.IsOnSuccessCalled);
            Assert.False(BaseObservableAspect.IsOnExceptionCalled);
        }
        
        [Fact]
        public void MultipleMethodBoundaryAspects_OnSuccess_Exception_NotHandledByHandler()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var result = Assert.Throws<AccessViolationException>(() => proxy.ThrowsOnSuccess());

            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
            Assert.True(BaseObservableAspect.IsOnSuccessCalled);
            Assert.False(BaseObservableAspect.IsOnExceptionCalled);
        }
        
        [Fact]
        public void MultipleMethodBoundaryAspects_OnException_Exception_NotHandledByHandler()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var result = Assert.Throws<AccessViolationException>(() => proxy.ThrowsOnException());

            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
            Assert.False(BaseObservableAspect.IsOnSuccessCalled);
            Assert.True(BaseObservableAspect.IsOnExceptionCalled);
        }
        
        [Fact]
        public void MultipleMethodBoundaryAspects_OnExit_Exception_NotHandledByHandler()
        {
            // Arrange
            var proxy = (IAppService)_aspectWeaver.Weave(new AppService(), typeof(IAppService));

            // Act
            var result = Assert.Throws<AccessViolationException>(() => proxy.ThrowsOnExit());

            // Assert
            Assert.True(BaseObservableAspect.IsOnEntryCalled);
            Assert.True(BaseObservableAspect.IsOnExitCalled);
            Assert.True(BaseObservableAspect.IsOnSuccessCalled);
            Assert.False(BaseObservableAspect.IsOnExceptionCalled);
        }
    }

    public interface IAppService
    {
        [BypassAspect]
        void BypassAspectSingle();

        [BypassAspect]
        void ThrowsExceptionNotHandled();

        [ThrowNewExceptionInOnException]
        void ThrowsNewException();

        [SwallowExceptionReplaceStringValueHandler]
        string SwallowExceptionWithVal();

        [OnEntryExceptionThrower]
        [SwallowExceptionVoidReturn]
        string ThrowsOnEntry();

        [OnSuccessExceptionThrower]
        [SwallowExceptionVoidReturn]
        string ThrowsOnSuccess();

        [OnExceptionExceptionThrower]
        [SwallowExceptionVoidReturn]
        string ThrowsOnException();

        [OnExitExceptionThrower]
        [SwallowExceptionVoidReturn]
        string ThrowsOnExit();
    }

    public class AppService : IAppService
    {
        public void BypassAspectSingle()
        {
            return;
        }

        public void ThrowsExceptionNotHandled()
        {
            throw new ArgumentException();
        }

        public void ThrowsNewException()
        {
            throw new ArgumentException();
        }

        public string SwallowExceptionWithVal()
        {
            throw new ArgumentException();
        }

        public string ThrowsOnEntry()
        {
            return "success";
        }

        public string ThrowsOnSuccess()
        {
            return "success";
        }

        public string ThrowsOnException()
        {
            throw new ArgumentException();
        }

        public string ThrowsOnExit()
        {
            return "success";
        }
    }

    public abstract class BaseObservableAspect : MethodBoundaryAspect
    {
        public static bool IsOnEntryCalled { get; private set; }
        public static bool IsOnSuccessCalled { get; private set; }
        public static bool IsOnExitCalled { get; private set; }
        public static bool IsOnExceptionCalled { get; private set; }
        
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            IsOnEntryCalled = true;
            OnEntryInternal(pipeline);
        }

        protected virtual void OnEntryInternal(IInvocationPipeline pipeline) { }
        
        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            IsOnSuccessCalled = true;
            OnSuccessInternal(pipeline);
        }

        protected virtual void OnSuccessInternal(IInvocationPipeline pipeline) { }
        
        public override void OnExit(IInvocationPipeline pipeline)
        {
            IsOnExitCalled = true;
            OnExitInternal(pipeline);
        }

        protected virtual void OnExitInternal(IInvocationPipeline pipeline) { }

        public override void OnException(IInvocationPipeline pipeline)
        {
            IsOnExceptionCalled = true;
            OnExceptionInternal(pipeline);
        }
        
        protected virtual void OnExceptionInternal(IInvocationPipeline pipeline) { }

        public static void ResetFlags()
        {
            IsOnEntryCalled = IsOnSuccessCalled = IsOnExitCalled = IsOnExceptionCalled = false;
        }
    }

    public class BypassAspect : BaseObservableAspect
    { }

    public class ThrowNewExceptionInOnException : BaseObservableAspect
    {
        protected override void OnExceptionInternal(IInvocationPipeline pipeline)
        {
            pipeline.ThrowException(new InvalidOperationException());
        }
    }

    public class SwallowExceptionVoidReturn : BaseObservableAspect
    {
        protected override void OnExceptionInternal(IInvocationPipeline pipeline)
        {
            pipeline.Return();
        }
    }
    
    public class SwallowExceptionReplaceStringValueHandler : BaseObservableAspect
    {
        protected override void OnExceptionInternal(IInvocationPipeline pipeline)
        {
            pipeline.Return("swallowed");
        }
    }

    public class OnEntryExceptionThrower : BaseObservableAspect
    {
        protected override void OnEntryInternal(IInvocationPipeline pipeline)
        {
            throw new AccessViolationException();
        }
    }

    public class OnSuccessExceptionThrower : BaseObservableAspect
    {
        protected override void OnSuccessInternal(IInvocationPipeline pipeline)
        {
            throw new AccessViolationException();
        }
    }

    public class OnExceptionExceptionThrower : BaseObservableAspect
    {
        protected override void OnExceptionInternal(IInvocationPipeline pipeline)
        {
            throw new AccessViolationException();
        }
    }

    public class OnExitExceptionThrower : BaseObservableAspect
    {
        protected override void OnExitInternal(IInvocationPipeline pipeline)
        {
            throw new AccessViolationException();
        }
    }
}