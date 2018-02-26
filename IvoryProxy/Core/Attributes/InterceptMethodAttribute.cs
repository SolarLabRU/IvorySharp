using System;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Attributes
{
    /// <summary>
    /// Атрибут позволяет отметить методы доступные для перехвата.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InterceptMethodAttribute : Attribute
    {
        /// <summary>
        /// Тип перехватчика для обработки метода.
        /// </summary>
        public Type InterceptorType { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InterceptMethodAttribute"/>.
        /// </summary>
        /// <param name="interceptorType">Тип обработчика.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="interceptorType"/> равен <c>null</c>.</exception>
        /// <exception cref="IvoryProxyException">Если обработчик с типом <paramref name="interceptorType"/> не реализует интерфейс <see cref="IMethodInterceptor"/>.</exception>
        public InterceptMethodAttribute(Type interceptorType)
        {
            if (interceptorType == null)
                throw new ArgumentNullException(nameof(interceptorType));
            
            if (!typeof(IMethodInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new IvoryProxyException(
                    $"Тип перехватчика должен реализовывать интерфейс '{typeof(IMethodInterceptor)}'. " +
                    $"Исходный тип: {interceptorType.FullName} . " +
                    $"Параметр: {nameof(interceptorType)}");
            }

            InterceptorType = interceptorType;
        }
    }
}