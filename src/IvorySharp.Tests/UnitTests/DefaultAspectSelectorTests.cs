using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Selection;
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
            var result = _selector.HasAnyAspect(typeof(IZeroTypeAspectsService));

            // Assert
            Assert.False(result, $"Expected: False | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_SingleTypeAspect_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(ISingleTypeAspectService));

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_MultipleTypeAspects_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(ISingleTypeAspectService));

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_ZeroMethodAspects_HasAnyAspect_Returns_False()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.ZeroMethodAspects)));

            // Assert
            Assert.False(result, $"Expected: False | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_SingleMethodAspect_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.SingleMethodAspect)));

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_MultipleMethodAspects_HasAnyAspect_Returns_True()
        {
            // Arrange
            var result = _selector.HasAnyAspect(typeof(IHasMethodAspects)
                .GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)));

            // Assert
            Assert.True(result, $"Expected: True | Method ({nameof(IAspectSelector.HasAnyAspect)})");
        }

        [Fact]
        public void Interface_ZeroMethodAspects_GetAspectDeclarations_Returns_EmptyArray()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.ZeroMethodAspects)));

            // Assert
            Assert.Empty(aspects);
        }

        [Fact]
        public void Interface_SingleMethodAspect_GetAspectDeclarations_Returns_Aspect()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.SingleMethodAspect)));

            // Assert
            Assert.Single(aspects);
        }

        [Fact]
        public void Interface_MultipleMethodAspect_GetAspectDeclarations_Returns_Aspects()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)));

            // Assert
            Assert.Equal(2, aspects.Count());
        }

        [Fact]
        public void Interface_MultipleMethodAspect_GetAspectDeclarations_Respects_AspectsType()
        {
            // Arrange
            var bypassAspects =
                _selector.SelectAspectDeclarations<BypassAspect>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)));

            var anotherBypassAspects =
                _selector.SelectAspectDeclarations<AnotherBypassAspects>(
                    typeof(IHasMethodAspects).GetMethod(nameof(IHasMethodAspects.TwoMethodAspects)));

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
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(IZeroTypeAspectsService));

            // Assert
            Assert.Empty(aspects);
        }

        [Fact]
        public void Interface_SingleTypeAspect_GetAspectDeclarations_Returns_Aspect()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(ISingleTypeAspectService));

            // Assert
            Assert.Single(aspects);
        }

        [Fact]
        public void Interface_MultipleTypeAspect_GetAspectDeclarations_Returns_Aspects()
        {
            // Arrange
            var aspects =
                _selector.SelectAspectDeclarations<MethodAspect>(typeof(ITwoTypeAspectsService));

            // Assert
            Assert.Equal(2, aspects.Count());
        }

        [Fact]
        public void Interface_MultipleTypeAspect_GetAspectDeclarations_Respects_AspectsType()
        {
            // Arrange
            var bypassAspects =
                _selector.SelectAspectDeclarations<BypassAspect>(typeof(ITwoTypeAspectsService));

            var anotherBypassAspects =
                _selector.SelectAspectDeclarations<AnotherBypassAspects>(
                    typeof(ITwoTypeAspectsService));

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