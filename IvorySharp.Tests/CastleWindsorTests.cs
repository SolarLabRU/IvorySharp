//using System;
//using Castle.MicroKernel.Registration;
//using Castle.Windsor;
//using IvorySharp.Aspects;
//using IvorySharp.Aspects.Configuration;
//using IvorySharp.Aspects.Pipeline;
//using IvorySharp.CastleWindsor.Aspects.Integration;
//using Xunit;
//
//namespace IvorySharp.Tests
//{
//    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
//    public class EnableAspectsAttribute : Attribute { }
//    
//    public class CastleWindsorTests
//    {
//        private WindsorContainer _diContainer;
//
//        public CastleWindsorTests()
//        {
//            CastleObservableAspect.ResetFlags();
//            
//            _diContainer = new WindsorContainer();
//
//            AspectsConfigurator
//                .UseContainer(new WindsorAspectsContainer(_diContainer))
//                .Initialize(configuration =>
//                {
//                    configuration.UseExplicitWeavingAttribute<EnableAspectsAttribute>();
//                });
//            
//            _diContainer.Register(
//                Component.For<ISomeService>()
//                    .ImplementedBy<SomeService>());
//        }
//
//        [Fact]
//        public void SingleAspect_MethodBoundaries_Called()
//        {
//            // Arrange
//            var service = _diContainer.Resolve<ISomeService>();
//
//            // Act
//            service.BypassVoidMethod();
//            
//            // Assert
//            Assert.True(CastleObservableAspect.IsOnEntryCalled);
//            Assert.True(CastleObservableAspect.IsOnSuccessCalled);
//            Assert.True(CastleObservableAspect.IsOnExitCalled);
//        }
//        
//        [Fact]
//        public void SingleAspect_MethodBoundaries_ResultReplaced()
//        {
//            // Arrange
//            var service = _diContainer.Resolve<ISomeService>();
//
//            // Act
//            var r = service.ReplaceResult();
//            
//            // Assert
//            Assert.Equal("hello world", r);
//            Assert.True(CastleObservableAspect.IsOnEntryCalled);
//            Assert.True(CastleObservableAspect.IsOnSuccessCalled);
//            Assert.True(CastleObservableAspect.IsOnExitCalled);
//        }
//        
//        [Fact]
//        public void SingleAspect_MethodBoundaries_ExceptionSwallowed()
//        {
//            // Arrange
//            var service = _diContainer.Resolve<ISomeService>();
//
//            // Act
//            var r = service.SwallowExceptionSetResult();
//            
//            // Assert
//            Assert.Equal(42, r);
//            Assert.True(CastleObservableAspect.IsOnEntryCalled);
//            Assert.True(CastleObservableAspect.IsOnExceptionCalled);
//            Assert.True(CastleObservableAspect.IsOnExitCalled);
//        }
//    }
//
//    [EnableAspects]
//    public interface ISomeService
//    {
//        [CastleBypassAspect]
//        void BypassVoidMethod();
//
//        [ReplaceResultOnSuccees]
//        string ReplaceResult();
//
//        [SwallowExceptionBoundary]
//        int SwallowExceptionSetResult();
//    }
//
//    public class SomeService : ISomeService
//    {
//        public void BypassVoidMethod()
//        {
//            
//        }
//
//        public string ReplaceResult()
//        {
//            return string.Empty;
//        }
//
//        public int SwallowExceptionSetResult()
//        {
//            throw new ArgumentException();
//        }
//    }
//    
//    public abstract class CastleObservableAspect : MethodBoundaryAspect
//    {
//        public static bool IsOnEntryCalled { get; private set; }
//        public static bool IsOnSuccessCalled { get; private set; }
//        public static bool IsOnExitCalled { get; private set; }
//        public static bool IsOnExceptionCalled { get; private set; }
//        
//        public override void OnEntry(IInvocationPipeline pipeline)
//        {
//            IsOnEntryCalled = true;
//            OnEntryInternal(pipeline);
//        }
//
//        protected virtual void OnEntryInternal(IInvocationPipeline pipeline) { }
//        
//        public override void OnSuccess(IInvocationPipeline pipeline)
//        {
//            IsOnSuccessCalled = true;
//            OnSuccessInternal(pipeline);
//        }
//
//        protected virtual void OnSuccessInternal(IInvocationPipeline pipeline) { }
//        
//        public override void OnExit(IInvocationPipeline pipeline)
//        {
//            IsOnExitCalled = true;
//            OnExitInternal(pipeline);
//        }
//
//        protected virtual void OnExitInternal(IInvocationPipeline pipeline) { }
//
//        public override void OnException(IInvocationPipeline pipeline)
//        {
//            IsOnExceptionCalled = true;
//            OnExceptionInternal(pipeline);
//        }
//        
//        protected virtual void OnExceptionInternal(IInvocationPipeline pipeline) { }
//
//        public static void ResetFlags()
//        {
//            IsOnEntryCalled = IsOnSuccessCalled = IsOnExitCalled = IsOnExceptionCalled = false;
//        }
//    }
//    
//    public class CastleBypassAspect : CastleObservableAspect { }
//
//    public class ReplaceResultOnSuccees : CastleObservableAspect
//    {
//        protected override void OnSuccessInternal(IInvocationPipeline pipeline)
//        {
//            pipeline.Return("hello world");
//        }
//    }
//
//    public class SwallowExceptionBoundary : CastleObservableAspect
//    {
//        protected override void OnExceptionInternal(IInvocationPipeline pipeline)
//        {
//            pipeline.Return(42);
//        }
//    }
//}