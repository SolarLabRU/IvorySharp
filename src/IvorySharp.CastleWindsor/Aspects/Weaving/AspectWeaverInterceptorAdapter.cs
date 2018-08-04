using System.Reflection;
using System.Runtime.ExceptionServices;
using Castle.DynamicProxy;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Configuration;
using IvorySharp.CastleWindsor.Core;

namespace IvorySharp.CastleWindsor.Aspects.Weaving
{
    /// <summary>
    /// Адаптирует механизм перехвата под внутренний механизм библиотеки Dynamic.Proxy.
    /// И прокси использоваться тоже будут кастловские.
    /// </summary>
    public class AspectWeaverInterceptorAdapter : IInterceptor
    {
        private readonly AspectWeaveInterceptor _aspectWeaveInterceptor;

        /// <summary>
        /// Адаптер для обработчика вызовов <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="components">Конфигурация аспектов.</param>
        public AspectWeaverInterceptorAdapter(IComponentsStore components)
        {
            _aspectWeaveInterceptor = new AspectWeaveInterceptor(
                components.AspectWeavePredicate, components.AspectPipelineExecutor, components.AspectInitializer);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            try
            {
                var adaptedInvocation = new InvocationAdapter(invocation);

                _aspectWeaveInterceptor.Intercept(adaptedInvocation);

                if (adaptedInvocation.ReturnValue != null)
                    invocation.ReturnValue = adaptedInvocation.ReturnValue;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();

                throw;
            }
        }
    }
}