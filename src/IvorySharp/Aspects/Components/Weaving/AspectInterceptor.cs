using System;
using IvorySharp.Aspects.Components.Caching;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;

// ReSharper disable FieldCanBeMadeReadOnly.Local (Reason: readonly access slower)

namespace IvorySharp.Aspects.Components.Weaving
{
    /// <summary>
    /// Перехватчик для применения аспектов.
    /// </summary>
    internal class AspectInterceptor : IInterceptor
    {
        private readonly Func<InvocationContext, bool> _cachedWeavePredicate;
        private readonly IPipelineExecutor _aspectPipelineExecutor;
        private readonly IAspectFactory _aspectFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectInterceptor"/>.
        /// </summary>
        /// <param name="weaveablePredicate">Предикат применения аспектов.</param>
        /// <param name="aspectPipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="aspectFactory">Фабрика аспектов.</param>
        public AspectInterceptor(
            IAspectWeavePredicate weaveablePredicate,
            IPipelineExecutor aspectPipelineExecutor, 
            IAspectFactory aspectFactory)
        {
            _aspectPipelineExecutor = aspectPipelineExecutor;
            _aspectFactory = aspectFactory;
            _cachedWeavePredicate = Cache.CreateProducer(
                ctx => weaveablePredicate.IsWeaveable(ctx.Method, ctx.DeclaringType, ctx.TargetType), 
                InvocationContext.ByMethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (!_cachedWeavePredicate(invocation.Context))
            {
                invocation.Proceed();
                return;
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
        }   
    }
}