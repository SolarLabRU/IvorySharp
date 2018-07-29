using System;
using System.Collections.Generic;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;

// ReSharper disable FieldCanBeMadeReadOnly.Local (Reason: readonly access slower)

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Перехватчик для применения аспектов.
    /// </summary>
    public class AspectWeaveInterceptor : IInterceptor
    {
        private MethodAspectDependencyInjector _aspectDependencyInjector;
        
        private Func<InvocationContext, List<MethodBoundaryAspect>> _methodBoundariesAspectsMemoizedProvider;
        private Func<InvocationContext, MethodInterceptionAspect> _methodInterceptionAspectMemoizedProvider;
        private Func<InvocationContext, bool> _isWeavableMemoizedProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="settings">Конфигурация аспектов.</param>
        public AspectWeaveInterceptor(IAspectsWeavingSettings settings)
        {
            _aspectDependencyInjector = new MethodAspectDependencyInjector(settings.ServiceProvider);

            _methodBoundariesAspectsMemoizedProvider = Memoizer.Memoize(
                MethodAspectFactory.Instance.CreateMethodBoundaryAspects,
                InvocationContext.MethodComparer);

            _methodInterceptionAspectMemoizedProvider = Memoizer.Memoize(
                MethodAspectFactory.Instance.CreateMethodInterceptionAspect,
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

            var methodInterceptAspect = _methodInterceptionAspectMemoizedProvider(invocation.Context);
            var methodBoundaryAspects = _methodBoundariesAspectsMemoizedProvider(invocation.Context);

            _aspectDependencyInjector.InjectDependencies(methodInterceptAspect);
            methodInterceptAspect.Initialize();
            
            foreach (var aspect in methodBoundaryAspects)
            {
                _aspectDependencyInjector.InjectDependencies(aspect);
                aspect.Initialize();
            }
            
            MethodAspectsExecutor.Instance.ExecuteAspects(invocation, methodBoundaryAspects, methodInterceptAspect);
        }   
    }
}