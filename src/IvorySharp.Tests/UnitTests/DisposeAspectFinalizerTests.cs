using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Finalize;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    public class DisposeAspectFinalizerTests
    {
        [Fact]
        public void DisposeMethod_Called_If_Aspect_Implements_IDisposable()
        {
            // Arrange
            var aspect = new DisposableAspect();
            var finalizer = new DisposeAspectFinalizer();
            
            // Act
            finalizer.Finalize(aspect);

            // Assert
            Assert.True(aspect.IsDisposed);
        }

        #region Aspects

        private class DisposableAspect : MethodBoundaryAspect, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        #endregion
    }
}