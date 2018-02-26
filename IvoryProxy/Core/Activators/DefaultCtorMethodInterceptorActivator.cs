using System;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Activators
{
    /// <summary>
    /// Активатор перехватчиков на основе конструктора по умолчанию.
    /// </summary>
    internal class DefaultCtorMethodInterceptorActivator : IMethodInterceptorActivator
    {      
        /// <inheritdoc />
        public IMethodInterceptor CreateInstance(Type interceptorType)
        {
            if (interceptorType == null)
                throw new ArgumentNullException(nameof(interceptorType));
            
            if (!typeof(IMethodInterceptor).IsAssignableFrom(interceptorType) || !interceptorType.IsClass)
            {
                throw new IvoryProxyException(
                    $"Тип перехватчика должен быть классом и реализовывать интерфейс '{typeof(IMethodInterceptor)}'. " +
                    $"Исходный тип: {interceptorType.FullName} . " +
                    $"Параметр: {nameof(interceptorType)}");
            }

            try
            {
                return (IMethodInterceptor) Activator.CreateInstance(interceptorType);
            }
            catch (Exception e)
            {
                throw new IvoryProxyException(
                    $"Возникло исключение при создании экземпляра перехватчика '{interceptorType.FullName}'.", e);
            }
        }
    }
}