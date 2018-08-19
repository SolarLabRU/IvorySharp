using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="DefaultAspectDependencyProvider"/>
    /// </summary>
    public class DefaultAspectDependencyProviderTests
    {
        private readonly IAspectDependencyProvider _aspectDependencyProvider;

        public DefaultAspectDependencyProviderTests()
        {
            _aspectDependencyProvider = new DefaultAspectDependencyProvider();
        }

        [Fact]
        public void NoPropertyDependencies_GetPropertyDependencies_Returns_EmptyArray()
        {
            // Act
            var deps = _aspectDependencyProvider.GetPropertyDependencies(typeof(ZeroPropertyDependenciesAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void SinglePropertyDependency_GetPropertyDependencies_Returns_CorrectDependency()
        {
            // Act
            var deps = _aspectDependencyProvider.GetPropertyDependencies(typeof(SinglePublicPropertyDependencyAspect));

            // Assert
            Assert.Single(deps);
            Assert.Equal(nameof(SinglePublicPropertyDependencyAspect.Dependency), deps.ElementAt(0).Property.Name);
        }

        [Fact]
        public void MultiplePropertyDependencies_GetPropertyDependencies_Returns_CorrectDependencies()
        {
            // Act
            var deps = _aspectDependencyProvider.GetPropertyDependencies(typeof(MultiplePublicPropertyDependenciesAspect));

            // Assert
            Assert.Equal(2, deps.Count());
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency1), deps.ElementAt(0).Property.Name);
            Assert.Equal(nameof(MultiplePublicPropertyDependenciesAspect.Dependency2), deps.ElementAt(1).Property.Name);
        }

        [Fact]
        public void NotPublicPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Act
            var deps = _aspectDependencyProvider.GetPropertyDependencies(typeof(SinglePrivatePropertyDependencyAspect));

            // Assert
            Assert.Empty(deps);
        }

        [Fact]
        public void NoSetPropertyDependency_GetPropertyDependencies_Returns_Result_Without_Dependency()
        {
            // Act
            var deps = _aspectDependencyProvider.GetPropertyDependencies(typeof(SinglePublicNoSetPropertyDependencyAspect));

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
            [Dependency]
            public object Dependency { get; set; }
        }

        private class MultiplePublicPropertyDependenciesAspect : MethodBoundaryAspect
        {
            [Dependency]
            public object Dependency1 { get; set; }

            [Dependency]
            public object Dependency2 { get; set; }
        }

        private class SinglePrivatePropertyDependencyAspect : MethodBoundaryAspect
        {
            [Dependency]
            private object Dependency { get; set; }
        }

        private class SinglePublicNoSetPropertyDependencyAspect : MethodBoundaryAspect
        {
            [Dependency]
            public object Dependency { get; }
        }

        #endregion
    }
}