using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
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
                dependencyProvider.Object.ToInstanceHolder(),
                dependencySelector.Object.ToInstanceHolder());
            
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