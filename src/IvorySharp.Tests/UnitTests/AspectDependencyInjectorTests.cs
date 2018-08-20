using System;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="DefaultAspectDependencySelector"/>.
    /// </summary>
    public class DefaultAspectDependencySelectorTests
    {
        private readonly DefaultAspectDependencySelector _dependencySelector;

        public DefaultAspectDependencySelectorTests()
        {
            _dependencySelector = new DefaultAspectDependencySelector();
        }

        [Fact]
        public void NoPropertyDependencies_GetPropertyDependencies_Returns_EmptyArray()
        {
            // Act
            var deps = _dependencySelector.SelectPropertyDependencies(typeof(ZeroPropertyDependenciesAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void SinglePropertyDependency_GetPropertyDependencies_Returns_CorrectDependency()
        {
            // Act
            var deps = _dependencySelector.SelectPropertyDependencies(typeof(SinglePublicPropertyDependencyAspect));

            // Assert
            Assert.Single(deps);
            Assert.Equal(nameof(SinglePublicPropertyDependencyAspect.Dependency), deps.ElementAt(0).Property.Name);
        }

        [Fact]
        public void MultiplePropertyDependencies_GetPropertyDependencies_Returns_CorrectDependencies()
        {
            // Act
            var deps = _dependencySelector.SelectPropertyDependencies(
                typeof(MultiplePublicPropertyDependenciesAspect));

            // Assert
            Assert.Equal(2, deps.Length);
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency1), deps.ElementAt(0).Property.Name);
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency2), deps.ElementAt(1).Property.Name);
        }

        [Fact]
        public void NotPublicPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Act
            var deps = _dependencySelector.SelectPropertyDependencies(typeof(SinglePrivatePropertyDependencyAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void NoSetPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Act
            var deps = _dependencySelector.SelectPropertyDependencies(
                typeof(SinglePublicNoSetPropertyDependencyAspect));

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
            [Dependency] public object Dependency { get; set; }
        }

        private class MultiplePublicPropertyDependenciesAspect : MethodBoundaryAspect
        {
            [Dependency] public object Dependency1 { get; set; }

            [Dependency] public object Dependency2 { get; set; }
        }

        private class SinglePrivatePropertyDependencyAspect : MethodBoundaryAspect
        {
            [Dependency] private object Dependency { get; set; }
        }

        private class SinglePublicNoSetPropertyDependencyAspect : MethodBoundaryAspect
        {
            [Dependency] public object Dependency { get; }
        }

        #endregion
    }

    /// <summary>
    /// Набор тестов для <see cref="AspectDependencyInjector"/>
    /// </summary>
    public class AspectDependencyInjectorTests
    {
        [Fact]
        public void SinglePropertyDependency_InjectPropertyDependencies_Should_Inject_Service()
        {
            // Arrange
            var dependencySelector = new Mock<IAspectDependencySelector>();

            dependencySelector.Setup(s => s.SelectPropertyDependencies(It.IsAny<Type>()))
                .Returns(new[]
                {
                    new AspectPropertyDependency(
                        new DependencyAttribute(),
                        typeof(SinglePublicPropertyDependencyAspect)
                            .GetProperty(nameof(SinglePublicPropertyDependencyAspect.Dependency))),
                });

            var dependencyService = new object();
            var dependencyProvider = new Mock<IDependencyProvider>();

            dependencyProvider.Setup(c => c.GetService(typeof(object))).Returns(dependencyService);

            var injector = new AspectDependencyInjector(
                dependencyProvider.Object.ToProvider(),
                dependencySelector.Object.ToProvider());
            
            var aspect = new SinglePublicPropertyDependencyAspect();

            // Act
            injector.InjectPropertyDependencies(aspect);

            // Assert
            Assert.Equal(dependencyService, aspect.Dependency);
        }

        #region Aspects

        private class SinglePublicPropertyDependencyAspect : MethodBoundaryAspect
        {
            [Dependency] public object Dependency { get; set; }
        }

        #endregion
    }
}