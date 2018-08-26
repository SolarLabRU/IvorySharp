using Castle.DynamicProxy;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace IvorySharp.Integration.CastleWindsor.Aspects.Weaving
{
    /// <summary>
    /// Адаптирует механизм перехвата под внутренний механизм библиотеки Dynamic.Proxy.
    /// И прокси использоваться тоже будут кастловские.
    /// </summary>
    /// <typeparam name="TService">Тип сервиса.</typeparam>
    /// <typeparam name="TImplementation">Тип реализации.</typeparam>
    public class WeavedInterceptor<TService, TImplementation> : IInterceptor
    {
        private readonly InvocationInterceptor _invocationInterceptor;

        /// <summary>
        /// Адаптер для обработчика вызовов <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="components">Компоненты библиотеки.</param>
        public WeavedInterceptor(IComponentsStore components)
        {
            var dataProviderFactory = components.WeaveDataProviderFactory.Get();         
            var dataProvider = dataProviderFactory.Create(typeof(TService), typeof(TImplementation));

            _invocationInterceptor = new InvocationInterceptor(
                dataProvider, 
                components.AspectDependencyInjector, 
                components.AspectFinalizer);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            object Proceed(object target, object[] arguments)
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            }
            
            var signature = new InvocationSignature(
                invocation.Method,
                invocation.MethodInvocationTarget, 
                typeof(TService),
                typeof(TImplementation), 
                invocation.Method.GetInvocationType());

            invocation.ReturnValue = _invocationInterceptor.Intercept(
                signature, Proceed, invocation.Arguments, 
                invocation.InvocationTarget, invocation.Proxy);
        }
    }
}