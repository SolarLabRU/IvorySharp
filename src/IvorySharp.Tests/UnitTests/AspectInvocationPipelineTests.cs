using System;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Tests.Assets.Invocations;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="InvocationPipeline"/>
    /// </summary>
    public class AspectInvocationPipelineTests
    {
        private readonly BypassInvocation _voidMethodReturnInvocation;
        private readonly BypassInvocation _returnTenMethodInvocation;

        public AspectInvocationPipelineTests()
        {
            _voidMethodReturnInvocation =
                new BypassInvocation(typeof(IService), new Service(), nameof(IService.Method));
            _returnTenMethodInvocation =
                new BypassInvocation(typeof(IService), new Service(), nameof(IService.ReturnTen));
        }

        [Fact]
        public void Call_Rethow_Should_SetException_And_ChangeFlow_To_Rethow()
        {
            // Arrage
            var pipeline = CreatePipeline(_voidMethodReturnInvocation);
            var exception = new ArgumentException();

            // Act
            pipeline.RethrowException(exception);

            // Assert
            Assert.Equal(exception, pipeline.CurrentException);
            Assert.Equal(FlowBehavior.RethrowException, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_Throw_Should_SetException_And_ChangeFlow_To_Throw()
        {
            // Arrage
            var pipeline = CreatePipeline(_voidMethodReturnInvocation);
            var exception = new ArgumentException();

            // Act
            pipeline.ThrowException(exception);

            // Assert
            Assert.Equal(exception, pipeline.CurrentException);
            Assert.Equal(FlowBehavior.ThrowException, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_Return_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // Arrange
            var pipeline = CreatePipeline(_returnTenMethodInvocation);

            // Act
            pipeline.Return();

            // Assert
            Assert.Equal(0, pipeline.Invocation.ReturnValue);
            Assert.Equal(FlowBehavior.Return, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_ReturnValue_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // Arrange
            var pipeline = CreatePipeline(_returnTenMethodInvocation);

            // Act
            pipeline.ReturnValue(15);

            // Assert
            Assert.Equal(15, pipeline.Invocation.ReturnValue);
            Assert.Equal(FlowBehavior.Return, pipeline.FlowBehavior);
        }

        private static InvocationPipelineBase CreatePipeline(IInvocation invocation)
        {
            return new InvocationPipeline(
                    new InvocationSignature(invocation.Method,
                        invocation.TargetMethod,
                        invocation.DeclaringType,
                        invocation.TargetType,
                        invocation.InvocationType), null, null)
                .Init(invocation);
        }

        private interface IService
        {
            void Method();

            int ReturnTen();
        }

        private class Service : IService
        {
            public void Method()
            {
            }

            public int ReturnTen()
            {
                return 10;
            }
        }
    }
}