using IvoryProxy.Core.Proxies;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Генератор прокси-объектов.
    /// </summary>
    public class IvoryProxyGenerator : IProxyGenerator
    {
        /// <inheritdoc />
        public IIvoryProxy<T> CreateInterfaceProxy<T>(T source) 
            where T : class
        {
#if NETCOREAPP2_0          
            return IvoryDispatchProxy<T>.CreateProxy(source);           
#endif
        }
    }
}