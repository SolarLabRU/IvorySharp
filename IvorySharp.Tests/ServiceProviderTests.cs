using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.CastleWindsor.Aspects.Dependency;
using IvorySharp.CastleWindsor.Aspects.Integration;
using IvorySharp.Proxying;
using IvorySharp.SimpleInjector.Aspects.Integration;
using IvorySharp.Tests.Aspects;
using IvorySharp.Tests.Helpers;
using IvorySharp.Tests.Services;
using SimpleInjector;
using Xunit;

namespace IvorySharp.Tests
{
    public class ServiceProviderTests
    {
        private readonly IServiceProvider _simpleInjectorServiceProvider;
        private readonly IServiceProvider _windsorServiceProvider;

        public ServiceProviderTests()
        {
            var simpleInjectorContainer = new Container();

            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(simpleInjectorContainer))
                .Initialize();

            simpleInjectorContainer.Register<ISingleBoundaryAspectService, SingleBoundaryAspectService>();
            simpleInjectorContainer.Register<IDependencyService, DependencyService>();

            _simpleInjectorServiceProvider = new SimpleInjectorServiceProvider(simpleInjectorContainer);

            var windsorContainer = new WindsorContainer();

            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(windsorContainer))
                .Initialize();

            windsorContainer.Register(
                Component
                    .For<ISingleBoundaryAspectService>()
                    .ImplementedBy<SingleBoundaryAspectService>());

            windsorContainer.Register(
                Component
                    .For<IDependencyService>()
                    .ImplementedBy<DependencyService>());

            _windsorServiceProvider =
                new CastleWindsor.Aspects.Dependency.WindsorServiceProvider(windsorContainer.Kernel);
        }

        [Fact]
        public void SimpleInjector_GetService_Weaved_ReturnsProxy()
        {
            // Arrange
            var service = _simpleInjectorServiceProvider.GetService<ISingleBoundaryAspectService>();

            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.NotNull(service as InterceptDispatchProxy);
        }

        [Fact]
        public void SimpleInjector_GetTransparentService_Weaved_ReturnsTransparentInstance()
        {
            // Arrange
            var service = _simpleInjectorServiceProvider.GetTransparentService<ISingleBoundaryAspectService>();

            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.Null(service as InterceptDispatchProxy);
        }

        [Fact]
        public void SimpleInjector_GetTransparentService_NotWeaved_ReturnsInstance()
        {
            // Arrange
            var service = _simpleInjectorServiceProvider.GetTransparentService<IDependencyService>();

            // Act & Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.Null(service as InterceptDispatchProxy);
        }

        [Fact]
        public void CastleWindsor_GetService_Weaved_ReturnsProxy()
        {
            // Arrange
            var service = _windsorServiceProvider.GetService<ISingleBoundaryAspectService>();

            // Assert
            Assert.True(ProxyUtil.IsProxy(service));
        }

        [Fact]
        public void CastleWindsor_GetTransparentService_Weaved_ReturnsTransparentInstance()
        {
            // Arrange
            var service = _windsorServiceProvider.GetTransparentService<ISingleBoundaryAspectService>();

            // Act
            service.BypassEmptyMethod();

            // Assert
            Assert.False(ProxyUtil.IsProxy(service));
        }

        [Fact]
        public void CastleWindsor_GetTransparentService_Noteaved_ReturnsInstance()
        {
            // Arrange
            var service = _windsorServiceProvider.GetTransparentService<IDependencyService>();

            // Act & Assert
            Assert.False(ProxyUtil.IsProxy(service));
        }
    }
}