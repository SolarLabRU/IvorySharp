using System;
using System.Linq;
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
        private readonly IComponentProvider<IAspectFactory<MethodBoundaryAspect>> _boundaryAspectFactoryProvider;
        private readonly IComponentProvider<IAspectFactory<MethodInterceptionAspect>> _interceptionAspectFactoryProvider;
        private readonly IComponentProvider<IInvocationPipelineFactory> _pipelineFactoryProvider;
        private readonly IComponentProvider<IAspectWeavePredicate> _aspectWeavePredicateProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationInterceptor"/>.
        /// </summary>
        public InvocationInterceptor(
            IComponentProvider<IAspectFactory<MethodBoundaryAspect>> boundaryAspectFactoryProvider,
            IComponentProvider<IAspectFactory<MethodInterceptionAspect>> interceptionAspectFactoryProvider,
            IComponentProvider<IInvocationPipelineFactory> pipelineFactoryProvider,
            IComponentProvider<IAspectWeavePredicate> aspectWeavePredicateProvider)
        {
            _boundaryAspectFactoryProvider = boundaryAspectFactoryProvider;
            _interceptionAspectFactoryProvider = interceptionAspectFactoryProvider;
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

            var pipelineFactory = _pipelineFactoryProvider.Get();       
            
            var boundaryAspects = _boundaryAspectFactoryProvider.Get().CreateAspects(invocation);       
            var interceptAspect = _interceptionAspectFactoryProvider.Get().CreateAspects(invocation)
                .SingleOrDefault();

            if (interceptAspect == null)
                interceptAspect = BypassMethodAspect.Instance;
            
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