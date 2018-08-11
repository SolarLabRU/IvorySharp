using System;
using IvorySharp.Core;
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
                new InvocationContext(
                    Array.Empty<object>(), 
                    method, 
                    new ReturnThisService(),
                    new FakeProxy(), 
                    typeof(IReturnThisService),
                    typeof(ReturnThisService)
                ), (instance, args) => ((IReturnThisService)instance).ReturnThis());

            // Act
            invocation.Proceed();

            // Assert
            Assert.IsType<FakeProxy>(invocation.ReturnValue);
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

        #endregion
    }
}