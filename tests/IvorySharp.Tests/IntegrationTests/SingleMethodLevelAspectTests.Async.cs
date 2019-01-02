using System;
using System.Threading.Tasks;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using IvorySharp.Tests.Assets;
using Xunit;

namespace IvorySharp.Tests.IntegrationTests
{
    public class SingleMethodLevelAspectAsyncTests
    {
        private readonly Weaved<IAsyncService, AsyncService> _serviceProvider;

        public SingleMethodLevelAspectAsyncTests()
        {
            IComponentsStore componentsStore = new DefaultComponentsStore(NullDependencyProvider.Instance);
            _serviceProvider = new Weaved<IAsyncService, AsyncService>(componentsStore);
        }


        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public async Task OnSuccess_ChangeReturnResult_By_ReturnValueProp(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act
            var result = await service.ShouldReturnArgumentPlusOneAsync(10);

            // Assert
            Assert.Equal(11, result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public async Task BypassAspect_Should_Not_SwallowException_If_It_WasThrown_By_Method(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.ShouldThrowArgumentExceptionAsync());
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public async Task SwallowException_In_OnExceptionBlock_ShouldReturnDefault(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act
            var result = await service.ShouldNotThrowArgumentExceptionAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public async Task OnEntry_ReturnValue_ShouldNotTriggerMethod_Invocation(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act
            var result = await service.ShouldReturnNewObjectInsteadOfNullAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public async Task ThrowException_In_OnSuccessBlock_ShouldFaultInvocation(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.ShouldThrowArgumentNullExceptionAsync(10));
        }

        #region Aspects

        private class SwallowExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.Return();
            }
        }

        private class BypassAspect : MethodBoundaryAspect
        {
        }

        private class IncrementValueOnSuccessAspects : MethodBoundaryAspect
        {
            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                var value = (int) pipeline.CurrentReturnValue;
                pipeline.Return(value + 1);
            }
        }

        private class ReturnObjectOnEntryAspect : MethodBoundaryAspect
        {
            public override void OnEntry(IInvocationPipeline pipeline)
            {
                pipeline.Return(new object());
            }
        }

        private class ThrowArgumentNullExceptionOnSuccess : MethodBoundaryAspect
        {
            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                throw new ArgumentNullException();
            }
        }

        #endregion

        #region Services

        public interface IAsyncService
        {
            [IncrementValueOnSuccessAspects]
            Task<int> ShouldReturnArgumentPlusOneAsync(int argument);

            [BypassAspect]
            Task ShouldThrowArgumentExceptionAsync();

            [SwallowExceptionAspect]
            Task<int> ShouldNotThrowArgumentExceptionAsync();

            [ReturnObjectOnEntryAspect]
            Task<object> ShouldReturnNewObjectInsteadOfNullAsync();

            [ThrowArgumentNullExceptionOnSuccess]
            Task<int> ShouldThrowArgumentNullExceptionAsync(int argument);
        }

        public class AsyncService : IAsyncService
        {
            public async Task<int> ShouldReturnArgumentPlusOneAsync(int argument)
            {
                return await Task.FromResult(argument);
            }

            public async Task ShouldThrowArgumentExceptionAsync()
            {
                await Task.FromException(new ArgumentException());
            }

            public async Task<int> ShouldNotThrowArgumentExceptionAsync()
            {
                return await Task.FromException<int>(new ArgumentException());
            }

            public async Task<object> ShouldReturnNewObjectInsteadOfNullAsync()
            {
                return await Task.FromResult<object>(null);
            }

            public async Task<int> ShouldThrowArgumentNullExceptionAsync(int argument)
            {
                return await Task.FromResult(argument);
            }
        }

        #endregion
    }
}