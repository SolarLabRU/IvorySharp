using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Aspects;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="DefaultAspectPreInitializer{T}"/>.
    /// </summary>
    public class DefaultAspectPreInitializerTests
    {
        [Fact]
        public void PrepareAspects_ShouldRemove_DuplicatedAspects()
        {
            // Arrange
            var declarations = new []
            {
                MethodAspectDeclaration<MethodBoundaryAspect>.FromType(new ReturnDefaultValueAspect(BoundaryType.Entry)  { Order = 0 }, null),
                MethodAspectDeclaration<MethodBoundaryAspect>.FromType(new ObservableAspect{ Order = 2}, null), 
                MethodAspectDeclaration<MethodBoundaryAspect>.FromMethod(new ObservableAspect{ Order = 1}, null),
            };

            var collector = CreateAspectCollector(declarations);
            var orderer = CreateAspectOrderStrategy<MethodBoundaryAspect, int>(a => a.Order);
            var preInitializer = new DefaultAspectPreInitializer<MethodBoundaryAspect>(
                collector.ToProvider(),
                orderer.ToProvider());
             
            
            // Act
            var aspects = preInitializer.PrepareAspects(context: null);

            // Assert
            Assert.Equal(2, aspects.Length);
            Assert.Equal(declarations.ElementAt(0).MethodAspect, aspects.ElementAt(0)); // 1й был удален. тк. его приоритет ниже
            Assert.Equal(declarations.ElementAt(2).MethodAspect, aspects.ElementAt(1));
        }

        private IAspectDeclarationCollector CreateAspectCollector(
            MethodAspectDeclaration<MethodBoundaryAspect>[] boundaryAspectDeclarations)
        {
            var mock = new Mock<IAspectDeclarationCollector>();

            mock.Setup(c => c.CollectAspectDeclarations<MethodBoundaryAspect>(It.IsAny<IInvocationContext>()))
                .Returns(boundaryAspectDeclarations);
            
            return mock.Object;
        }

        private IAspectOrderStrategy CreateAspectOrderStrategy<TAspect, TProperty>(Func<TAspect, TProperty> orderer) 
            where TAspect : OrderableMethodAspect
        {
            var mock = new Mock<IAspectOrderStrategy>();

            mock.Setup(c => c.Order<TAspect>(It.IsAny<IEnumerable<TAspect>>()))
                .Returns<IEnumerable<TAspect>>(args => args.OrderBy(orderer));

            return mock.Object;
        }
    }
}