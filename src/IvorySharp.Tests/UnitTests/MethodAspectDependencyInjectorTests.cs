using System;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Dependency;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="MethodAspectDependencyInjector"/>
    /// </summary>
    public class MethodAspectDependencyInjectorTests
    {
        [Fact]
        public void SinglePropertyDependency_InjectPropertyDependencies_Should_Inject_Service()
        {
            // Arrange
            var dependencyService = new object();
            var dependencyProvider = new Mock<IDependencyProvider>();

            dependencyProvider.Setup(c => c.GetService(typeof(object))).Returns(dependencyService);

            var injector = new MethodAspectDependencyInjector(dependencyProvider.Object);
            var aspect = new SinglePublicPropertyDependencyAspect();

            // Act
            injector.InjectPropertyDependencies(aspect);

            // Assert
            Assert.Equal(dependencyService, aspect.Dependency);
        }

        [Fact]
        public void NoPropertyDependencies_GetPropertyDependencies_Returns_EmptyArray()
        {
            // Arrange
            var injector = new MethodAspectDependencyInjector(null);

            // Act
            var deps = injector.GetPropertyDependencies(typeof(ZeroPropertyDependenciesAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void SinglePropertyDependency_GetPropertyDependencies_Returns_CorrectDependency()
        {
            // Arrange
            var injector = new MethodAspectDependencyInjector(null);

            // Act
            var deps = injector.GetPropertyDependencies(typeof(SinglePublicPropertyDependencyAspect));

            // Assert
            Assert.Single(deps);
            Assert.Equal(nameof(SinglePublicPropertyDependencyAspect.Dependency), deps.ElementAt(0).Property.Name);
        }

        [Fact]
        public void MultiplePropertyDependencies_GetPropertyDependencies_Returns_CorrectDependencies()
        {
            // Arrange
            var injector = new MethodAspectDependencyInjector(null);

            // Act
            var deps = injector.GetPropertyDependencies(typeof(MultiplePublicPropertyDependenciesAspect));

            // Assert
            Assert.Equal(2, deps.Length);
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency1), deps.ElementAt(0).Property.Name);
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency2), deps.ElementAt(1).Property.Name);
        }

        [Fact]
        public void NotPublicPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Arrange
            var injector = new MethodAspectDependencyInjector(null);

            // Act
            var deps = injector.GetPropertyDependencies(typeof(SinglePrivatePropertyDependencyAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void NoSetPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Arrange
            var injector = new MethodAspectDependencyInjector(null);

            // Act
            var deps = injector.GetPropertyDependencies(typeof(SinglePublicNoSetPropertyDependencyAspect));

            // Assert
            Assert.Empty(deps);
        }

        #region Aspects

        private class ZeroPropertyDependenciesAspect : MethodBoundaryAspect
        {
            public object Prop { get; set; }
        }

        private class SinglePublicPropertyDependencyAspect : MethodBoundaryAspect
        {
            [InjectDependency]
            public object Dependency { get; set; }
        }

        private class MultiplePublicPropertyDependenciesAspect : MethodBoundaryAspect
        {
            [InjectDependency]
            public object Dependency1 { get; set; }

            [InjectDependency]
            public object Dependency2 { get; set; }
        }

        private class SinglePrivatePropertyDependencyAspect : MethodBoundaryAspect
        {
            [InjectDependency]
            private object Dependency { get; set; }
        }

        private class SinglePublicNoSetPropertyDependencyAspect : MethodBoundaryAspect
        {
            [InjectDependency]
            public object Dependency { get; }
        }

        #endregion
    }
}