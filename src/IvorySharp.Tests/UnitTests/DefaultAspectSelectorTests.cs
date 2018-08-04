using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Selection;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="DefaultAspectSelector"/>
    /// </summary>
    public class DefaultAspectSelectorTests
    {
        private readonly DefaultAspectSelector _selector;

        public DefaultAspectSelectorTests()
        {
            _selector = new DefaultAspectSelector();
        }

        #region Tests

        [Fact]
        public void Interface_ZeroTypeAspects_HasAnyAspect_Returns_False()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IZeroTypeAspectsService), includeAbstract: false);

            // Assert
            Assert.False(result, $"Expected: False | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_SingleTypeAspect_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(ISingleTypeAspectService), includeAbstract: false);

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_MultipleTypeAspects_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(ISingleTypeAspectService), includeAbstract: false);

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_ZeroMethodAspects_HasAnyAspect_Returns_False()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.ZeroMethodAspects)), includeAbstract: false);

            // Assert
            Assert.False(result, $"Expected: False | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_SingleMethodAspect_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.SingleMethodAspect)), includeAbstract: false);

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_MultipleMethodAspects_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)), includeAbstract: false);

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_ZeroMethodAspects_GetAspectDeclarations_Returns_EmptyArray()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.ZeroMethodAspects)), includeAbstract: false);

            // Assert
            Assert.Empty(aspects);
        }

        [Fact]
        public void Interface_SingleMethodAspect_GetAspectDeclarations_Returns_Aspect()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.SingleMethodAspect)), includeAbstract: false);

            // Assert
            Assert.Single(aspects);
        }

        [Fact]
        public void Interface_MultipleMethodAspect_GetAspectDeclarations_Returns_Aspects()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)), includeAbstract: false);

            // Assert
            Assert.Equal(2, aspects.Length);
        }

        [Fact]
        public void Interface_MultipleMethodAspect_GetAspectDeclarations_Respects_AspectsType()
        {
            // Arrange
            var bypassAspects =
                _selector.SelectAspectDeclarations<BypassAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)), includeAbstract: false);

            var anotherBypassAspects =
                _selector.SelectAspectDeclarations<AnotherBypassAspects>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)), includeAbstract: false);

            Assert.Single(bypassAspects);
            Assert.IsType<BypassAspect>(bypassAspects.ElementAt(0).MethodAspect);

            Assert.Single(anotherBypassAspects);
            Assert.IsType<AnotherBypassAspects>(anotherBypassAspects.ElementAt(0).MethodAspect);
        }

        [Fact]
        public void Interface_ZeroTypeAspects_GetAspectDeclarations_Returns_EmptyArray()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(IZeroTypeAspectsService), includeAbstract: false);

            // Assert
            Assert.Empty(aspects);
        }

        [Fact]
        public void Interface_SingleTypeAspect_GetAspectDeclarations_Returns_Aspect()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(ISingleTypeAspectService), includeAbstract: false);

            // Assert
            Assert.Single(aspects);
        }

        [Fact]
        public void Interface_MultipleTypeAspect_GetAspectDeclarations_Returns_Aspects()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(ITwoTypeAspectsService), includeAbstract: false);

            // Assert
            Assert.Equal(2, aspects.Length);
        }

        [Fact]
        public void Interface_MultipleTypeAspect_GetAspectDeclarations_Respects_AspectsType()
        {
            // Arrange
            var bypassAspects =
                _selector.SelectAspectDeclarations<BypassAspect>(typeof(ITwoTypeAspectsService), includeAbstract: false);

            var anotherBypassAspects =
                _selector.SelectAspectDeclarations<AnotherBypassAspects>(typeof(ITwoTypeAspectsService), includeAbstract: false);

            Assert.Single(bypassAspects);
            Assert.IsType<BypassAspect>(bypassAspects.ElementAt(0).MethodAspect);

            Assert.Single(anotherBypassAspects);
            Assert.IsType<AnotherBypassAspects>(anotherBypassAspects.ElementAt(0).MethodAspect);
        }

        #endregion

        #region Aspects

        private class BypassAspect : MethodBoundaryAspect
        {         
        }

        private class AnotherBypassAspects : MethodBoundaryAspect
        {       
        }

        #endregion

        #region Services

        private interface IHasMethodAspects
        {
            void ZeroMethodAspects();

            [BypassAspect]
            void SingleMethodAspect();

            [BypassAspect, AnotherBypassAspects]
            void TwoMethodAspects();
        }

        private interface IZeroTypeAspectsService
        {
        }

        [BypassAspect]
        private interface ISingleTypeAspectService
        {         
        }

        [BypassAspect, AnotherBypassAspects]
        private interface ITwoTypeAspectsService
        {
            
        }

        #endregion
    }
}