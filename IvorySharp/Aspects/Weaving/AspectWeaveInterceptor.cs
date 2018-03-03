using System;
using System.Reflection;
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
        /// <summary>
        /// Селектор аспектов.
        /// </summary>
        public IMethodAspectSelector MethodAspectSelector { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="methodAspectSelector">Селектор аспектов.</param>
        public AspectWeaveInterceptor(IMethodAspectSelector methodAspectSelector)
        {
            MethodAspectSelector = methodAspectSelector ?? throw new ArgumentNullException(nameof(methodAspectSelector));
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (IsNotWeaveable(invocation))
            {
                invocation.Proceed();
                return;
            }

            var methodBoundaryAspects = MethodAspectSelector.GetMethodBoundaryAspects(invocation);
            if (methodBoundaryAspects.IsEmpty())
            {
                invocation.Proceed();
                return;
            }

            var pipelineInjector = new InvocationPipelineInjector(methodBoundaryAspects);
            pipelineInjector.Inject(invocation);
        }

        /// <summary>
        /// Возвращает признак того, что для вызова нельзя применить внедрение аспектов.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Признак того, что для вызова нельзя применить внедрение аспектов.</returns>
        private bool IsNotWeaveable(IInvocation invocation)
        {
            return invocation.Context.InstanceDeclaringType.GetCustomAttribute<SupressAspectAttribute>(inherit: false) != null ||
                   invocation.Context.Method.GetCustomAttribute<SupressAspectAttribute>(inherit: false) != null;
        }
    }
}