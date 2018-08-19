using Castle.DynamicProxy;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Integration.CastleWindsor.Core;

namespace IvorySharp.Integration.CastleWindsor.Aspects.Weaving
{
    /// <summary>
    /// Адаптирует механизм перехвата под внутренний механизм библиотеки Dynamic.Proxy.
    /// И прокси использоваться тоже будут кастловские.
    /// </summary>
    /// <typeparam name="TService">Тип сервиса.</typeparam>
    public class WeavedInterceptor<TService> : IInterceptor
    {
        private readonly InvocationInterceptor _invocationFacade;

        /// <summary>
        /// Адаптер для обработчика вызовов <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="components">Компоненты библиотеки.</param>
        public WeavedInterceptor(IComponentsStore components)
        {
            _invocationFacade = new InvocationInterceptor(
                components.AspectFactory, components.PipelineFactory, components.AspectWeavePredicate);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = _invocationFacade.Intercept(
                new InvocationAdapter(invocation, typeof(TService)));
        }
    }
}