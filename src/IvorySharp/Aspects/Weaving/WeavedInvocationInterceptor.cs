using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Класс, содержащий логику перехвата вызываемого метода.
    /// </summary>
    internal class WeavedInvocationInterceptor
    {
        private readonly IAspectFactory _aspectFactory;
        private readonly IPipelineExecutor _aspectPipelineExecutor;
        private readonly IAspectWeavePredicate _aspectWeavePredicate;
        
        public WeavedInvocationInterceptor(IAspectFactory aspectFactory, IPipelineExecutor aspectPipelineExecutor, IAspectWeavePredicate aspectWeavePredicate)
        {
            _aspectFactory = aspectFactory;
            _aspectPipelineExecutor = aspectPipelineExecutor;
            _aspectWeavePredicate = aspectWeavePredicate;
        }

        /// <summary>
        /// Выполняет перехват вызова исходного метода с применением аспектов.
        /// </summary>
        /// <param name="invocation">Модель исходного вызова.</param>
        /// <returns>Результат вызова метода.</returns>
        internal object InterceptInvocation(IInvocation invocation)
        {
            if (!_aspectWeavePredicate.IsWeaveable(
                invocation.Context.Method,
                invocation.Context.DeclaringType,
                invocation.Context.TargetType))
            {
                return invocation.Proceed();
            }

            var boundaryAspects = _aspectFactory.CreateBoundaryAspects(invocation.Context);
            var interceptAspect = _aspectFactory.CreateInterceptionAspect(invocation.Context);

            _aspectPipelineExecutor.ExecutePipeline(
                new AspectInvocationPipeline(invocation, boundaryAspects, interceptAspect));

            foreach (var aspect in boundaryAspects)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (aspect is IDisposable ds1)
                    ds1.Dispose();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (interceptAspect is IDisposable ds2)
                ds2.Dispose();

            return invocation.Context.ReturnValue;
        }
    }
}