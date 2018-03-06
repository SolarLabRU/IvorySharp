using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Перехватчик для применения аспектов.
    /// </summary>
    public class AspectWeaveInterceptor : IInterceptor
    {
        private MethodBoundaryAspectsInjector _aspectsInjector;     
        private Func<InvocationContext, MethodBoundaryAspect[]> _methodBoundariesMemoizedProvider;
        private Func<InvocationContext, bool> _isWeavableMemoizedProvider;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="settings">Конфигурация аспектов.</param>
        public AspectWeaveInterceptor(IAspectsWeavingSettings settings)
        {
            _aspectsInjector = new MethodBoundaryAspectsInjector(settings);

            _methodBoundariesMemoizedProvider = Memoizer.Memoize(
                MethodAspectSelector.Instance.GetMethodBoundaryAspects,
                InvocationContext.MethodComparer);

            _isWeavableMemoizedProvider = Memoizer.Memoize(
                ctx=> AspectWeaver.IsWeavable(ctx, settings),
                InvocationContext.MethodComparer);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (!_isWeavableMemoizedProvider(invocation.Context))
            {
                invocation.Proceed();
                return;
            }

            var methodBoundaryAspects = _methodBoundariesMemoizedProvider(invocation.Context);
            if (methodBoundaryAspects.IsEmpty())
            {
                invocation.Proceed();
                return;
            }

            _aspectsInjector.InjectAspects(invocation, methodBoundaryAspects);
        }   
    }
}