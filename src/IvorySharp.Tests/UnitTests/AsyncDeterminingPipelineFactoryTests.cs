using System.Linq;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Tests.Assets.Invocations;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для компонента <see cref="AsyncDeterminingPipelineFactory"/>.
    /// </summary>
    public class AsyncDeterminingPipelineFactoryTests
    {
        [Fact]
        public void IsAsync_ReturnsTrue_OnlyIf_Method_OnTargetType_IsAsync()
        {
            // Arrange
            var factory = AsyncDeterminingPipelineFactory.Instance;
            var service = new Service();

            // Act & Assert
            Assert.False(
                factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncVoid))));
            Assert.False(
                factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncTask))));
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IService), service,
                nameof(IService.NotAsyncTaskResult))));
            Assert.False(
                factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncResult))));

            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.AsyncTask))));
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service,
                nameof(IService.AsyncTaskResult))));
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.AsyncVoid))));
        }

        [Fact]
        public void IsAsync_ShoultNotFail_WithOverloaded_Methods()
        {
            // Arrange
            var factory = AsyncDeterminingPipelineFactory.Instance;
            var service = new OverloadService();
            var methods = typeof(IOverloadService).GetMethods()
                .Where(m => m.Name == nameof(IOverloadService.MethodAsync));

            // Act
            foreach (var method in methods)
            {
                Assert.True(factory.IsAsync(new BypassInvocation(typeof(IOverloadService), service, method)));
            }

            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IOverloadService), service,
                nameof(IOverloadService.NotAsyncMethod))));
        }

        [Fact]
        public void IsAsync_ShouldNotFail_WithExplicit_Methods()
        {
            // Arrange
            var factory = AsyncDeterminingPipelineFactory.Instance;
            var service = new ExplicitService();
            
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IExplicitServiceOne), service,
                nameof(IExplicitServiceOne.MethodAsync))));
            
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IExplicitServiceTwo), service,
                nameof(IExplicitServiceTwo.MethodAsync))));
        }

        #region Services

        private interface IService
        {
            void NotAsyncVoid();
            Task NotAsyncTask();
            Task<int> NotAsyncTaskResult();
            int NotAsyncResult();
            Task AsyncTask();
            Task<int> AsyncTaskResult();
            void AsyncVoid();
        }

        private class Service : IService
        {
            public void NotAsyncVoid()
            {
                return;
            }

            public Task NotAsyncTask()
            {
                return Task.CompletedTask;
            }

            public Task<int> NotAsyncTaskResult()
            {
                return Task.FromResult(1);
            }

            public int NotAsyncResult()
            {
                return 1;
            }

            public async Task AsyncTask()
            {
                await Task.CompletedTask;
            }

            public async Task<int> AsyncTaskResult()
            {
                return await Task.FromResult(1);
            }

            public async void AsyncVoid()
            {
                await Task.CompletedTask;
            }
        }

        private interface IOverloadService
        {
            Task MethodAsync(int arg1);
            Task MethodAsync(int arg1, int arg2);
            Task MethodAsync<T>(int arg1);
            Task<T> MethodAsync<T>(int arg1, int arg2);

            Task NotAsyncMethod(int arg1);
        }

        private class OverloadService : IOverloadService
        {
            public async Task MethodAsync(int arg1)
            {
                await Task.CompletedTask;
            }

            public async Task MethodAsync(int arg1, int arg2)
            {
                await Task.CompletedTask;
            }

            public async Task MethodAsync<T>(int arg1)
            {
                await Task.CompletedTask;
            }

            public async Task<T> MethodAsync<T>(int arg1, int arg2)
            {
                return await Task.FromResult(default(T));
            }

            public Task NotAsyncMethod(int arg1)
            {
                return Task.CompletedTask;
            }

            public async Task NotAsyncMethod()
            {
                await Task.CompletedTask;
            }

            public Task MethodAsync()
            {
                return Task.CompletedTask;
            }
        }

        private interface IExplicitServiceOne
        {
            Task MethodAsync(int arg1);
        }

        private interface IExplicitServiceTwo
        {
            Task MethodAsync(int arg1);
        }

        private class ExplicitService : IExplicitServiceOne, IExplicitServiceTwo
        {
            async Task IExplicitServiceOne.MethodAsync(int arg1)
            {
                await Task.CompletedTask;
            }

            Task IExplicitServiceTwo.MethodAsync(int arg1)
            {
                return Task.CompletedTask;
            }
        }

        #endregion
    }
}