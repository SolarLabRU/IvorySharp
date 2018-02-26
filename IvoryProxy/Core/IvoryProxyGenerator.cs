using System;
using IvoryProxy.Core.Exceptions;
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
            if (!typeof(T).IsInterface)
            {
                throw new IvoryProxyException(
                    $"Проксирование возможно только для интерфейсов. Тип '{typeof(T)}' не является интерфейсом");
            }

#if NETCOREAPP2_0
            return IvoryDispatchProxy<T>.CreateProxy(source);           
#elif NET461
            return new IvoryRealProxy<T>(source);
#endif
            throw new NotSupportedException("Не реализовано для текущей платформы");

        }
    }
}