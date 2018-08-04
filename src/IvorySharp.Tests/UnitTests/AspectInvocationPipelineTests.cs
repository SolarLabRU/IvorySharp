using System;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Assets;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="AspectInvocationPipeline"/>
    /// </summary>
    public class AspectInvocationPipelineTests
    {
        private readonly BypassInvocation _voidMethodReturnInvocation;
        private readonly BypassInvocation _zeroMethodReturnInvocation;
        private readonly BypassInvocation _nullMethodReturnInvocation;

        public AspectInvocationPipelineTests()
        {
            _voidMethodReturnInvocation = new BypassInvocation(typeof(IService), new Service(), nameof(IService.Method));
            _zeroMethodReturnInvocation = new BypassInvocation(typeof(IService), new Service(), nameof(IService.ReturnZero));
            _nullMethodReturnInvocation = new BypassInvocation(typeof(IService), new Service(), nameof(IService.ReturnNull));
        }

        [Fact]
        public void Call_Rethow_Should_SetException_And_ChangeFlow_To_Rethow()
        {
            // Arrage
            var pipeline = new AspectInvocationPipeline(_voidMethodReturnInvocation, null, null);
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
            var pipeline = new AspectInvocationPipeline(_voidMethodReturnInvocation, null, null);
            var exception = new ArgumentException();

            // Act
            pipeline.ThrowException(exception);

            // Assert
            Assert.Equal(exception, pipeline.CurrentException);
            Assert.Equal(FlowBehavior.ThrowException, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_ReturnDefault_ShouldSet_ReturnValue_To_Default_And_ChangeFlow_To_Return()
        {
            // TODO
        }

        [Fact]
        public void Call_Return_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // TODO
        }

        [Fact]
        public void Call_ReturnValue_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // TODO
        }

        [Fact]
        public void Call_ReturnValue_ShouldThrow_If_MethodReturnType_IsVoid()
        {
            // TODO
        }

        [Fact]
        public void Call_ReturnValue_ShouldThrow_If_ResultIsUncastable_To_Method_ReturnType()
        {
            // TODO
        }

        private interface IService
        {
            void Method();

            object ReturnNull();

            int ReturnZero();
        }

        private class Service : IService
        {
            public void Method()
            {
            }

            public object ReturnNull()
            {
                return default(object);
            }

            public int ReturnZero()
            {
                return 0;
            }
        }
    }
}
