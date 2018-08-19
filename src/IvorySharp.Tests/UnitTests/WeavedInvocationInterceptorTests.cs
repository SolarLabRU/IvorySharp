using System;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Tests.Assets.Invocations;
using Moq;
using Xunit;
using IInvocation = IvorySharp.Core.IInvocation;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="InvocationInterceptor"/>.
    /// </summary>
    public class WeavedInvocationInterceptorTests
    {
        private MethodBoundaryAspect[] _boundaryAspects;

        private readonly IAspectWeavePredicate _weavePredicateAlwaysTrue;
        private readonly IInvocationPipelineFactory _pipelineFactory;
        private readonly IAspectFactory _predefinedAspectsFactory;

        public WeavedInvocationInterceptorTests()
        {
            var truePredicateMock = new Mock<IAspectWeavePredicate>();

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<IInvocation>()))
                .Returns(true);

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(true);

            _weavePredicateAlwaysTrue = truePredicateMock.Object;

            var pipelineExecutorMock = new Mock<IInvocationPipelineExecutor>();
            var doNothingPipelineExecutor = pipelineExecutorMock.Object;

            var pipelineFactoryMock = new Mock<IInvocationPipelineFactory>();
            pipelineFactoryMock.Setup(c => c.CreateExecutor(It.IsAny<IInvocation>()))
                .Returns(doNothingPipelineExecutor);

            _pipelineFactory = pipelineFactoryMock.Object;
            
            var aspectInitializerMock = new Mock<IAspectFactory>();

            aspectInitializerMock.Setup(m => m.CreateBoundaryAspects(It.IsAny<IInvocationContext>()))
                .Returns(() => _boundaryAspects);

            _predefinedAspectsFactory = aspectInitializerMock.Object;
        }

        [Fact]
        public void DisposeMethod_Called_If_Aspect_Implements_IDisposable()
        {
            // Arrange
            var aspect = new DisposableAspect();
            var interceptor = new InvocationInterceptor(
                _predefinedAspectsFactory.ToProvider(), 
                _pipelineFactory.ToProvider(), 
                _weavePredicateAlwaysTrue.ToProvider());

            _boundaryAspects = new MethodBoundaryAspect[] { aspect };

            // Act

            Assert.False(aspect.IsDisposed);
            interceptor.Intercept(new BypassInvocation(typeof(IService), new Service(), nameof(IService.Method)));

            // Assert
            Assert.True(aspect.IsDisposed);
        }

        #region Services

        private interface IService
        {
            void Method();
        }

        private class Service : IService
        {
            public void Method()
            {
                
            }
        }

        #endregion

        #region Aspects

        private class DisposableAspect : MethodBoundaryAspect, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        #endregion
    }
}