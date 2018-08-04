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
    internal class AspectWeaveInterceptor : IInterceptor
    {
        private readonly Func<InvocationContext, bool> _cachedWeavePredicate;
        private readonly IMethodAspectPipelineExecutor _aspectPipelineExecutor;
        private readonly IMethodAspectInitializer _aspectInitializer;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="weaveablePredicate">Предикат применения аспектов.</param>
        /// <param name="aspectPipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="aspectInitializer"></param>
        public AspectWeaveInterceptor(
            IMethodAspectWeavePredicate weaveablePredicate,
            IMethodAspectPipelineExecutor aspectPipelineExecutor, 
            IMethodAspectInitializer aspectInitializer)
        {
            _aspectPipelineExecutor = aspectPipelineExecutor;
            _aspectInitializer = aspectInitializer;
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

            var boundaryAspects = _aspectInitializer.InitializeBoundaryAspects(invocation.Context);
            var interceptAspect = _aspectInitializer.InitializeInterceptionAspect(invocation.Context);

            _aspectPipelineExecutor.ExecutePipeline(
                new MethodAspectInvocationPipeline(invocation, boundaryAspects, interceptAspect));

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