using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline.Synchronous
{
    /// <summary>
    /// Пайплайн выполнения синхронного метода, включающий аспекты.
    /// </summary>
    internal class SyncAspectInvocationPipeline : SyncInvocationPipeline
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
        internal SyncAspectInvocationPipeline(
            IInvocation invocation, 
            MethodBoundaryAspect[] boundaryAspects,
            MethodInterceptionAspect interceptionAspect) 
            : base(invocation)
        {
            BoundaryAspects = boundaryAspects;
            InterceptionAspect = interceptionAspect;
        }
    }
}