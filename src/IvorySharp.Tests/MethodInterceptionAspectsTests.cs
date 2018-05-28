using IvorySharp.Aspects;
using IvorySharp.Tests.Helpers;
using IvorySharp.Tests.Services;
using IvorySharp.Tests.WeavingSettings;
using Xunit;

namespace IvorySharp.Tests
{
    /// <summary>
    /// Набор тестов для аспектов типа <see cref="MethodBoundaryAspect"/>.
    /// </summary>
    public class MethodInterceptionAspectsTests
    {
        private readonly WeavedServiceProvider<IInterceptAspectService, InterceptAspectService> _serviceProvider;

        public MethodInterceptionAspectsTests()
        {
            _serviceProvider = new WeavedServiceProvider<IInterceptAspectService, InterceptAspectService>(
                new ImplicitAspectsWeavingSettings());
        }

        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void InterceptMethod_SwallowException_ReturnsDefault(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _serviceProvider.GetService(storeType);

            // Act
            var result = service.ExceptionSwalloved();

            // Assert
            Assert.Equal(default(object), result);
        }
    }
}