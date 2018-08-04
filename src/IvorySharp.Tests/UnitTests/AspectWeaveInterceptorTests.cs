using System;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Tests.Assets;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="AspectWeaveInterceptor"/>.
    /// </summary>
    public class AspectWeaveInterceptorTests
    {
        private MethodBoundaryAspect[] _boundaryAspects;
        private MethodInterceptionAspect _interceptionAspect;

        private readonly IMethodAspectWeavePredicate _weavePredicateAlwaysTrue;
        private readonly IMethodAspectPipelineExecutor _doNothingPipelineExecutor;
        private readonly IMethodAspectInitializer _predefinedAspectsInitializer;

        public AspectWeaveInterceptorTests()
        {
            var truePredicateMock = new Mock<IMethodAspectWeavePredicate>();

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<MethodInfo>(), It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(true);

            truePredicateMock.Setup(p => p.IsWeaveable(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(true);

            _weavePredicateAlwaysTrue = truePredicateMock.Object;

            var pipelineExecutorMock = new Mock<IMethodAspectPipelineExecutor>();
            _doNothingPipelineExecutor = pipelineExecutorMock.Object;

            var aspectInitializerMock = new Mock<IMethodAspectInitializer>();

            aspectInitializerMock.Setup(m => m.InitializeBoundaryAspects(It.IsAny<InvocationContext>()))
                .Returns(() => _boundaryAspects);

            aspectInitializerMock.Setup(m => m.InitializeInterceptionAspect(It.IsAny<InvocationContext>()))
                .Returns(() => _interceptionAspect);

            _predefinedAspectsInitializer = aspectInitializerMock.Object;
        }

        [Fact]
        public void DisposeMethod_Called_If_Aspect_Implements_IDisposable()
        {
            // Arrange
            var aspect = new DisposableAspect();
            var interceptor = new AspectWeaveInterceptor(_weavePredicateAlwaysTrue, _doNothingPipelineExecutor, _predefinedAspectsInitializer);

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