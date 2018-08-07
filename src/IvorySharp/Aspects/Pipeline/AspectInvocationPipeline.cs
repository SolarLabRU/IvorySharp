using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Пайплайн выполнения, включающий аспекты.
    /// </summary>
    internal class AspectInvocationPipeline : InvocationPipeline
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
        /// Инициализирует экземпляр <see cref="AspectInvocationPipeline"/>.
        /// </summary>
        internal AspectInvocationPipeline(IInvocation invocation, IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, MethodInterceptionAspect interceptionAspect) 
            : base(invocation)
        {
            BoundaryAspects = boundaryAspects;
            InterceptionAspect = interceptionAspect;
        }
    }
}