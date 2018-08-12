using System;
using System.Reflection;
using IvorySharp.Aspects.Pipeline.Async;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Caching;
using IvorySharp.Comparers;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Реализация фабрики компонентов пайплайна. Выполняет определение компонентов
    /// на основе того, является ли метод асинхронным.
    /// </summary>
    internal class AsyncDeterminingPipelineFactory : IInvocationPipelineFactory
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="AsyncDeterminingPipelineFactory"/>.
        /// </summary>
        internal static readonly AsyncDeterminingPipelineFactory Instance
            = new AsyncDeterminingPipelineFactory();

        private readonly Func<MethodInfo, bool> _isAsyncCache;
        
        private AsyncDeterminingPipelineFactory()
        {
            _isAsyncCache = Memoizer.CreateProducer(m => m.IsAsync(), MethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public IInvocationPipeline CreatePipeline(
            IInvocation invocation,
            MethodBoundaryAspect[] boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
        {
            if (_isAsyncCache(invocation.TargetMethod))
                return new AsyncAspectInvocationPipeline(invocation, boundaryAspects, interceptionAspect);
            
            return new SyncAspectInvocationPipeline(invocation, boundaryAspects, interceptionAspect);
        }

        /// <inheritdoc />
        public IInvocationPipelineExecutor CreateExecutor(IInvocationContext context)
        {
            if (_isAsyncCache(context.TargetMethod))
                return AsyncAspectInvocationPipelineExecutor.Instance;
            
            return SyncAspectInvocationPipelineExecutor.Instance;
        }
    }
}