using IvorySharp.Aspects.Pipeline.Async;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Caching;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{  
    /// <summary>
    /// Реализация фабрики компонентов пайплайна. Выполняет определение компонентов
    /// на основе того, является ли метод асинхронным.
    /// </summary>
    internal sealed class AsyncDeterminingPipelineFactory : IInvocationPipelineFactory
    {
        private readonly IMethodCache _cache;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncDeterminingPipelineFactory"/>.
        /// </summary>
        /// <param name="methodCache">Кеш методов.</param>
        internal AsyncDeterminingPipelineFactory(IMethodCache methodCache)
        {
            _cache = methodCache;
        }
        
        /// <inheritdoc />
        public IInvocationPipeline CreatePipeline(
            IInvocation invocation,
            MethodBoundaryAspect[] boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
        {
            if (_cache.GetIsAsync(invocation.TargetMethod))
                return new AsyncInvocationPipeline(invocation, boundaryAspects, interceptionAspect);
            
            return new InvocationPipeline(invocation, boundaryAspects, interceptionAspect);
        }

        /// <inheritdoc />
        public IInvocationPipelineExecutor CreateExecutor(IInvocationContext context)
        {
            if (_cache.GetIsAsync(context.TargetMethod))
                return AsyncInvocationPipelineExecutor.Instance;
            
            return InvocationPipelineExecutor.Instance;
        }
    }
}