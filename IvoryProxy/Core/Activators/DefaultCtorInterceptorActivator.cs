using System;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Activators
{
    /// <summary>
    /// Активатор перехватчиков на основе конструктора по умолчанию.
    /// </summary>
    internal class DefaultCtorInterceptorActivator : IInterceptorActivator
    {      
        /// <inheritdoc />
        public IInterceptor CreateInstance(Type interceptorType)
        {
            if (interceptorType == null)
                throw new ArgumentNullException(nameof(interceptorType));
            
            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType) || !interceptorType.IsClass)
            {
                throw new IvoryProxyException(
                    $"Тип перехватчика должен быть классом и реализовывать интерфейс '{typeof(IInterceptor)}'. " +
                    $"Исходный тип: {interceptorType.FullName} . " +
                    $"Параметр: {nameof(interceptorType)}");
            }

            try
            {
                return (IInterceptor) Activator.CreateInstance(interceptorType);
            }
            catch (Exception e)
            {
                throw new IvoryProxyException(
                    $"Возникло исключение при создании экземпляра перехватчика '{interceptorType.FullName}'.", e);
            }
        }
    }
}