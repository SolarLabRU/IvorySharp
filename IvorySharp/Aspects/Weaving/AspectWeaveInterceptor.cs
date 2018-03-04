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
        private readonly IAspectsWeavingSettings _settings;
        private readonly MethodBoundaryAspectsInjector _aspectsInjector;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AspectWeaveInterceptor"/>.
        /// </summary>
        /// <param name="settings">Конфигурация аспектов.</param>
        public AspectWeaveInterceptor(IAspectsWeavingSettings settings)
        {
            _settings = settings;
            _aspectsInjector = new MethodBoundaryAspectsInjector(settings);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (!AspectWeaver.IsWeavable(invocation, _settings))
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

            _aspectsInjector.InjectAspects(invocation, methodBoundaryAspects);
        }   
    }
}