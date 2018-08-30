using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Selection;
using IvorySharp.Tests.Extensions;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="AspectOrderStrategy"/>
    /// </summary>
    public class DefaultAspectOrderStrategyTests
    {
        private readonly AspectOrderStrategy _orderStrategy;

        private readonly MethodBoundaryAspect[] _methodAspects;
        private readonly MethodBoundaryAspect[] _typeAspects;

        private IEnumerable<MethodBoundaryAspect> AggregateAspects => _methodAspects.Concat(_typeAspects).Shuffle();

        public DefaultAspectOrderStrategyTests()
        {
            _orderStrategy = new AspectOrderStrategy();

            _methodAspects = new[] {
                new EmptyAspect{ MulticastTarget = MethodAspectMulticastTarget.Method }, 
                new EmptyAspect{ MulticastTarget = MethodAspectMulticastTarget.Method }
            };

            _typeAspects = new[]
            {
                new EmptyAspect{ MulticastTarget = MethodAspectMulticastTarget.Type }, 
                new EmptyAspect{ MulticastTarget = MethodAspectMulticastTarget.Type }
            };
        }

        [Fact]
        public void TypeLevelAspects_ShouldBe_BeforeMethodAspects_In_OrderNotSet()
        {
            // Act
            var sorted = _orderStrategy.Order(AggregateAspects);

            // Assert
            Assert.Equal(_typeAspects.Concat(_methodAspects), sorted);
        }

        [Fact]
        public void Ordering_Should_Respect_Order()
        {
            // Arrange        
            int order = 0;
            foreach (var aspect in _methodAspects.Concat(_typeAspects))
            {
                aspect.Order = order++;
            }

            // Act
            var sorted = _orderStrategy.Order(AggregateAspects);

            // Assert
            Assert.Equal(_typeAspects.Concat(_methodAspects).OrderBy(a => a.Order), sorted);
        }

        #region Aspects

        private class EmptyAspect : MethodBoundaryAspect
        {
            
        }

        #endregion
    }
}