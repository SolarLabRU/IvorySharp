using System;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Core;
using IvorySharp.Tests.Assets;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="WeavedInvocationInterceptor"/>.
    /// </summary>
    public class WeavedInvocationInterceptorTests
    {
        private MethodBoundaryAspect[] _boundaryAspects;
        private MethodInterceptionAspect _interceptionAspect;

        private readonly IAspectWeavePredicate _weavePredicateAlwaysTrue;
        private readonly IPipelineExecutor _doNothingPipelineExecutor;
        private readonly IAspectFactory _predefinedAspectsFactory;

        public WeavedInvocationInterceptorTests()
        {
            var truePredicateMock = new Mock<IAspectWeavePredicate>();

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<MethodInfo>(), It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(true);

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(true);

            _weavePredicateAlwaysTrue = truePredicateMock.Object;

            var pipelineExecutorMock = new Mock<IPipelineExecutor>();
            _doNothingPipelineExecutor = pipelineExecutorMock.Object;

            var aspectInitializerMock = new Mock<IAspectFactory>();

            aspectInitializerMock.Setup(m => m.CreateBoundaryAspects(It.IsAny<InvocationContext>()))
                .Returns(() => _boundaryAspects);

            aspectInitializerMock.Setup(m => m.CreateInterceptionAspect(It.IsAny<InvocationContext>()))
                .Returns(() => _interceptionAspect);

            _predefinedAspectsFactory = aspectInitializerMock.Object;
        }

        [Fact]
        public void DisposeMethod_Called_If_Aspect_Implements_IDisposable()
        {
            // Arrange
            var aspect = new DisposableAspect();
            var interceptor = new WeavedInvocationInterceptor(_predefinedAspectsFactory, _doNothingPipelineExecutor, _weavePredicateAlwaysTrue);

            _boundaryAspects = new MethodBoundaryAspect[] { aspect };

            // Act

            Assert.False(aspect.IsDisposed);
            interceptor.InterceptInvocation(new BypassInvocation(typeof(IService), new Service(), nameof(IService.Method)));

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