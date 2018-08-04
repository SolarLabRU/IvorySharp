using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Configuration;
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
        private readonly IDependencyProvider _simpleInjectorDependencyProvider;
        private readonly IDependencyProvider _windsorDependencyProvider;

        public ServiceProviderTests()
        {
            var simpleInjectorContainer = new Container();

            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(simpleInjectorContainer))
                .Initialize();

            simpleInjectorContainer.Register<ISingleBoundaryAspectService, SingleBoundaryAspectService>();
            simpleInjectorContainer.Register<IDependencyService, DependencyService>();

            _simpleInjectorDependencyProvider = new SimpleInjectorDependencyProvider(simpleInjectorContainer);

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

            _windsorDependencyProvider =
                new CastleWindsor.Aspects.Dependency.WindsorDependencyProvider(windsorContainer.Kernel);
        }

        [Fact]
        public void SimpleInjector_GetService_Weaved_ReturnsProxy()
        {
            // Arrange
            var service = _simpleInjectorDependencyProvider.GetService<ISingleBoundaryAspectService>();

            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.NotNull(service as InterceptDispatchProxy);
        }

        [Fact]
        public void SimpleInjector_GetTransparentService_Weaved_ReturnsTransparentInstance()
        {
            // Arrange
            var service = _simpleInjectorDependencyProvider.GetTransparentService<ISingleBoundaryAspectService>();

            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.Null(service as InterceptDispatchProxy);
        }

        [Fact]
        public void SimpleInjector_GetTransparentService_NotWeaved_ReturnsInstance()
        {
            // Arrange
            var service = _simpleInjectorDependencyProvider.GetTransparentService<IDependencyService>();

            // Act & Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.Null(service as InterceptDispatchProxy);
        }

        [Fact]
        public void CastleWindsor_GetService_Weaved_ReturnsProxy()
        {
            // Arrange
            var service = _windsorDependencyProvider.GetService<ISingleBoundaryAspectService>();

            // Assert
            Assert.True(ProxyUtil.IsProxy(service));
        }

        [Fact]
        public void CastleWindsor_GetTransparentService_Weaved_ReturnsTransparentInstance()
        {
            // Arrange
            var service = _windsorDependencyProvider.GetTransparentService<ISingleBoundaryAspectService>();

            // Act
            service.BypassEmptyMethod();

            // Assert
            Assert.False(ProxyUtil.IsProxy(service));
        }

        [Fact]
        public void CastleWindsor_GetTransparentService_Noteaved_ReturnsInstance()
        {
            // Arrange
            var service = _windsorDependencyProvider.GetTransparentService<IDependencyService>();

            // Act & Assert
            Assert.False(ProxyUtil.IsProxy(service));
        }
    }
}