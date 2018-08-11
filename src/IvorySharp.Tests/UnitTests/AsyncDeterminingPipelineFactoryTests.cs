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
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncVoid))));
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncTask))));
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncTaskResult))));
            Assert.False(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.NotAsyncResult))));
            
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.AsyncTask))));
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.AsyncTaskResult))));
            Assert.True(factory.IsAsync(new BypassInvocation(typeof(IService), service, nameof(IService.AsyncVoid))));
        }
        
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
    }
}