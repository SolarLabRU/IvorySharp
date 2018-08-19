using System.Reflection;
using IvorySharp.Caching;
using IvorySharp.Proxying;

namespace IvorySharp.Benchmark.Proxy
{
    public class BypassProxy<TService> : IvoryProxy
    {
        private TService _service;
        
        protected internal override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return MethodCache.Instance.GetInvoker(targetMethod)(_service, args);
        }
        
        private void Initialize(TService service)
        {
            _service = service;
        }
        
        internal static object Create(TService service)
        {
            var transparentProxy = ProxyGenerator.Instance.CreateTransparentProxy(
                typeof(BypassProxy<TService> ), typeof(TService));
            
            var weavedProxy = (BypassProxy<TService> ) transparentProxy;

            weavedProxy.Initialize(service);

            return transparentProxy;
        }
    }
}