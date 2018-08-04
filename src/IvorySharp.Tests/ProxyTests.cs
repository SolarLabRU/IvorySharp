using IvorySharp.Tests.Aspects;
using IvorySharp.Tests.Helpers;
using IvorySharp.Tests.Services;
using IvorySharp.Tests.WeavingSettings;
using Xunit;

namespace IvorySharp.Tests
{
    public class ProxyTests
    {
        private readonly WeavedServiceProvider<IReturnThisService, ReturnThisService> _serviceProvider;
        
        public ProxyTests()
        {
            _serviceProvider = new WeavedServiceProvider<IReturnThisService, ReturnThisService>(
                new NullComponentsStore());
            
            ObservableBoundaryAspect.ClearCallings();
        }
        
        /// <summary>
        /// Выполняет проверку того, что возврат this из проксированного метода вернет прокси класс,
        /// а не внутренний тип прокси.
        /// </summary>
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void Return_This_From_Proxied_Method_Returns_Proxy(WeavedServiceStoreType serviceStoreType)
        {
            // Arrange
            var proxy = _serviceProvider.GetService(serviceStoreType);

            // Act
            var result = proxy.ReturnSelf();

            // Assert
            Assert.Same(proxy, result);
            
            AspectAssert.OnEntryCalled(typeof(BypassAspect));
            AspectAssert.OnExitCalled(typeof(BypassAspect));
        }
    }
}