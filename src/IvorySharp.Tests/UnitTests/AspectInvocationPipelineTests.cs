using System;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Exceptions;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Invocations;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="AspectInvocationPipeline"/>
    /// </summary>
    public class AspectInvocationPipelineTests
    {
        private readonly BypassInvocation _voidMethodReturnInvocation;
        private readonly BypassInvocation _returnTenMethodInvocation;
        private readonly BypassInvocation _nullMethodReturnInvocation;

        public AspectInvocationPipelineTests()
        {
            _voidMethodReturnInvocation = new BypassInvocation(typeof(IService), new Service(), nameof(IService.Method));
            _returnTenMethodInvocation = new BypassInvocation(typeof(IService), new Service(), nameof(IService.ReturnTen));
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
        public void Call_Return_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // Arrange
            var pipeline = new AspectInvocationPipeline(_returnTenMethodInvocation, null, null);
            
            // Act
            pipeline.Return();
            
            // Assert
            Assert.Equal(0, pipeline.Context.ReturnValue);
            Assert.Equal(FlowBehavior.Return, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_ReturnValue_ShouldSet_ReturnValue_And_ChangeFlow_To_Return()
        {
            // Arrange
            var pipeline = new AspectInvocationPipeline(_returnTenMethodInvocation, null, null);
            
            // Act
            pipeline.ReturnValue(15);
            
            // Assert
            Assert.Equal(15, pipeline.Context.ReturnValue);
            Assert.Equal(FlowBehavior.Return, pipeline.FlowBehavior);
        }

        [Fact]
        public void Call_ReturnValue_ShouldThrow_If_MethodReturnType_IsVoid()
        {
            // Arrange
            var pipeline = new AspectInvocationPipeline(_voidMethodReturnInvocation, null, null);
            
            // Act & Assert
            Assert.Throws<IvorySharpException>(() =>  pipeline.ReturnValue(15));
        }

        [Fact]
        public void Call_ReturnValue_ShouldThrow_If_ResultIsUncastable_To_Method_ReturnType()
        {
            // Arrange
            var pipeline = new AspectInvocationPipeline(_returnTenMethodInvocation, null, null);
            
            // Act & Assert
            Assert.Throws<IvorySharpException>(() =>  pipeline.ReturnValue(new object()));
        }

        private interface IService
        {
            void Method();

            object ReturnNull();

            int ReturnTen();
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

            public int ReturnTen()
            {
                return 10;
            }
        }
    }
}
