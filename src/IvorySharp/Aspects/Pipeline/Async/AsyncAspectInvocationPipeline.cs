using System.Collections.Generic;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline.Async
{
    /// <summary>
    /// Базовая модель асинхронного пайлпайна вызова.
    /// </summary>
    internal class AsyncAspectInvocationPipeline : AsyncInvocationPipeline
    {
        /// <summary>
        /// Аспекты закрепленные в точках до и после вызова основного метода.
        /// </summary>
        public IReadOnlyCollection<MethodBoundaryAspect> BoundaryAspects { get; }

        /// <summary>
        /// Аспект перехвата вызова метода.
        /// </summary>
        public MethodInterceptionAspect InterceptionAspect { get; }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="SyncAspectInvocationPipeline"/>.
        /// </summary>
        internal AsyncAspectInvocationPipeline(
            IInvocation invocation,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
            : base(invocation)
        {
            BoundaryAspects = boundaryAspects;
            InterceptionAspect = interceptionAspect;
        }
    }
}