using IvorySharp.Proxying;

namespace IvorySharp.Benchmark.Proxy
{
    public class BypassProxy<TService> : IvoryProxy
    {
        private TService _service;
        
        protected internal override object Invoke(MethodInvocation invocation)
        {
            return invocation.MethodLambda(_service, invocation.Arguments);
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