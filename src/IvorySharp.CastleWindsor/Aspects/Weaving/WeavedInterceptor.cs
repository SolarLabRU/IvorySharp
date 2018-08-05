using Castle.DynamicProxy;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.CastleWindsor.Core;

namespace IvorySharp.CastleWindsor.Aspects.Weaving
{
    /// <summary>
    /// Адаптирует механизм перехвата под внутренний механизм библиотеки Dynamic.Proxy.
    /// И прокси использоваться тоже будут кастловские.
    /// </summary>
    public class WeavedInterceptor : IInterceptor
    {
        private readonly WeavedInvocationInterceptor _invocationFacade;

        /// <summary>
        /// Адаптер для обработчика вызовов <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="components">Компоненты библиотеки.</param>
        public WeavedInterceptor(IComponentsStore components)
        {
            _invocationFacade = new WeavedInvocationInterceptor(
                components.AspectFactory, components.AspectPipelineExecutor, components.AspectWeavePredicate);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = _invocationFacade.InterceptInvocation(new InvocationAdapter(invocation));
        }
    }
}