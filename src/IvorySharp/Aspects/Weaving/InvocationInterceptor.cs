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
        private readonly IComponentProvider<IAspectFactory> _aspectFactoryProvider;
        private readonly IComponentProvider<IInvocationPipelineFactory> _pipelineFactoryProvider;
        private readonly IComponentProvider<IAspectWeavePredicate> _aspectWeavePredicateProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationInterceptor"/>.
        /// </summary>
        public InvocationInterceptor(
            IComponentProvider<IAspectFactory>  aspectFactoryProvider,
            IComponentProvider<IInvocationPipelineFactory> pipelineFactoryProvider,
            IComponentProvider<IAspectWeavePredicate> aspectWeavePredicateProvider)
        {
            _aspectFactoryProvider = aspectFactoryProvider;
            _pipelineFactoryProvider = pipelineFactoryProvider;         
            _aspectWeavePredicateProvider = aspectWeavePredicateProvider;
        }

        /// <summary>
        /// Выполняет перехват вызова исходного метода с применением аспектов.
        /// </summary>
        /// <param name="invocation">Модель исходного вызова.</param>
        /// <returns>Результат вызова метода.</returns>
        internal object Intercept(IInvocation invocation)
        {
            var weavePredicate = _aspectWeavePredicateProvider.Get();
            
            if (!weavePredicate.IsWeaveable(invocation))
            {
                return invocation.Proceed();
            }

            var aspectFactory = _aspectFactoryProvider.Get();
            var pipelineFactory = _pipelineFactoryProvider.Get();
            
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