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
    public class AspectWeaverInterceptorAdapter : Castle.DynamicProxy.IInterceptor
    {
        private readonly AspectWeaveInterceptor _aspectWeaveInterceptor;
        
        public AspectWeaverInterceptorAdapter(IWeavingAspectsConfiguration configurations)
        {
            _aspectWeaveInterceptor = new AspectWeaveInterceptor(configurations);
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            var adaptedInvocation = new InvocationAdapter(invocation);
            
            _aspectWeaveInterceptor.Intercept(adaptedInvocation);
            invocation.ReturnValue = adaptedInvocation.ReturnValue;
        }
    }
}