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
        private readonly IWeavingAspectsConfiguration _configurations;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="aspectsConfiguration">Конфигурация аспектов.</param>
        public AspectWeaveInterceptor(IWeavingAspectsConfiguration aspectsConfiguration)
        {
            _configurations = aspectsConfiguration ?? throw new ArgumentNullException(nameof(aspectsConfiguration));
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (!IsWeaveable(invocation))
            {
                invocation.Proceed();
                return;
            }

            var methodBoundaryAspects = MethodAspectSelector.Instance.GetMethodBoundaryAspects(invocation);
            if (methodBoundaryAspects.IsEmpty())
            {
                invocation.Proceed();
                return;
            }

            MethodBoundaryAspectsInjector.Instance.InjectAspects(invocation, methodBoundaryAspects);
        }

        private bool IsWeaveable(IInvocation invocation)
        {
            return invocation.Context.InstanceDeclaringType.IsWeavable(_configurations);
        }    
    }
}