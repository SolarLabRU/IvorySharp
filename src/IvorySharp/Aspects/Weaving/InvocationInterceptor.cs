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

        private IAspectFactory _aspectFactory;
        private IInvocationPipelineFactory _pipelineFactory;
        private IAspectWeavePredicate _aspectWeavePredicate;
        
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
            if (_aspectWeavePredicate == null)
                _aspectWeavePredicate = _aspectWeavePredicateHolder.Get();
            
            if (!_aspectWeavePredicate.IsWeaveable(invocation))
                return invocation.Proceed();
            
            if (_aspectFactory == null)
                _aspectFactory = _aspectFactoryHolder.Get();

            if (_pipelineFactory == null)
                _pipelineFactory = _pipelineFactoryHolder.Get();
            
            var boundaryAspects = _aspectFactory.CreateBoundaryAspects(invocation);
            var interceptAspect = _aspectFactory.CreateInterceptionAspect(invocation);
            var executor = _pipelineFactory.CreateExecutor(invocation);
            var pipeline = _pipelineFactory.CreatePipeline(invocation, boundaryAspects, interceptAspect);    
            
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