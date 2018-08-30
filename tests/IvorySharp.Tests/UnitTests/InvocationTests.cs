using IvorySharp.Core;
using IvorySharp.Exceptions;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="Invocation"/>.
    /// </summary>
    public class InvocationTests
    {
        [Fact]
        public void Intercept_ReturningThisMethod_ShouldReturnProxy_InsteadOf_Instance()
        {
            // Arrange
            var method = typeof(IReturnThisService).GetMethod(nameof(IReturnThisService.ReturnThis));

            var invocation = new Invocation(
                InvocationArguments.Empty,
                method,
                typeof(IReturnThisService),
                typeof(ReturnThisService),
                new FakeProxy(), 
                new ReturnThisService(), 
                (instance, args) => ((IReturnThisService)instance).ReturnThis());
            
            // Act
            invocation.Proceed();

            // Assert
            Assert.IsType<FakeProxy>(invocation.ReturnValue);
        }
        
        [Fact]
        public void SetReturnValue_ToVoidMethod_Should_Throw()
        {
            // Arrange
            var method = typeof(ICastTestService).GetMethod(nameof(ICastTestService.ReturnVoid));

            var invocation = new Invocation(
                InvocationArguments.Empty,
                method,
                typeof(ICastTestService),
                typeof(CastTestService),
                null, 
                new CastTestService(),
                null);

            
            // Act & Assert
            Assert.Throws<IvorySharpException>(() => invocation.ReturnValue = new object());
        }

        [Fact]
        public void SetReturnValue_ShouldThrow_If_ResultIsUncastable_To_Method_ReturnType()
        {
            // Arrange
            var method = typeof(ICastTestService).GetMethod(nameof(ICastTestService.ReturnInt));

            var invocation = new Invocation(
                InvocationArguments.Empty,
                method,
                typeof(ICastTestService),
                typeof(CastTestService),
                null, 
                new CastTestService(),
                null);

            
            // Act & Assert
            Assert.Throws<IvorySharpException>(() => invocation.ReturnValue = new object());
        }

        #region Services

        private interface IReturnThisService
        {
            IReturnThisService ReturnThis();
        }

        private class ReturnThisService : IReturnThisService
        {
            public IReturnThisService ReturnThis()
            {
                return this;
            }
        }

        private class FakeProxy : ReturnThisService
        {        
        }
        
        private interface ICastTestService
        {
            void ReturnVoid();
            object ReturnObject();
            int ReturnInt();
        }
        
        private class CastTestService : ICastTestService
        {
            public void ReturnVoid()
            {
                return;
            }

            public object ReturnObject()
            {
                return new object();
            }

            public int ReturnInt()
            {
                return 10;
            }
        }

        #endregion
    }
}