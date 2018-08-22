using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Класс, содержащий логику перехвата вызываемого метода.
    /// </summary>
    internal sealed class InvocationInterceptor
    {
        private readonly IComponentHolder<IAspectFactory> _aspectFactoryHolder;
        private readonly IComponentHolder<IInvocationPipelineFactory> _pipelineFactoryHolder;
        private readonly IComponentHolder<IAspectWeavePredicate> _aspectWeavePredicateHolder;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationInterceptor"/>.
        /// </summary>
        public InvocationInterceptor(
            IComponentHolder<IAspectFactory>  aspectFactoryHolder,
            IComponentHolder<IInvocationPipelineFactory> pipelineFactoryHolder,
            IComponentHolder<IAspectWeavePredicate> aspectWeavePredicateHolder)
        {
            _aspectFactoryHolder = aspectFactoryHolder;
            _pipelineFactoryHolder = pipelineFactoryHolder;         
            _aspectWeavePredicateHolder = aspectWeavePredicateHolder;
        }

        /// <summary>
        /// Выполняет перехват вызова исходного метода с применением аспектов.
        /// </summary>
        /// <param name="invocation">Модель исходного вызова.</param>
        /// <returns>Результат вызова метода.</returns>
        internal object Intercept(IInvocation invocation)
        {
            var weavePredicate = _aspectWeavePredicateHolder.Get();
            
            if (!weavePredicate.IsWeaveable(invocation))
            {
                return invocation.Proceed();
            }

            var aspectFactory = _aspectFactoryHolder.Get();
            var pipelineFactory = _pipelineFactoryHolder.Get();
            
            var boundaryAspects = aspectFactory.CreateBoundaryAspects(invocation);
            var interceptAspect = aspectFactory.CreateInterceptionAspect(invocation);
            var executor = pipelineFactory.CreateExecutor(invocation);
            var pipeline = pipelineFactory.CreatePipeline(invocation, boundaryAspects, interceptAspect);    
            
            executor.ExecutePipeline(pipeline);

            foreach (var aspect in boundaryAspects)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (aspect is IDisposable ds1)
                    ds1.Dispose();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (interceptAspect is IDisposable ds2)
                ds2.Dispose();

            return invocation.ReturnValue;
        }
    }
}