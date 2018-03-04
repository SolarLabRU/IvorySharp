using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
        
        public AspectWeaverInterceptorAdapter(IAspectsWeavingSettings configurations)
        {
            _aspectWeaveInterceptor = new AspectWeaveInterceptor(configurations);
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