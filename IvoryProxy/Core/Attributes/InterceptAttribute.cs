using System;

namespace IvoryProxy.Core.Attributes
{
    /// <summary>
    /// Атрибут позволяет отметить методы доступные для перехвата.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class InterceptAttribute : Attribute
    {
        /// <summary>
        /// Тип перехватчика для обработки метода.
        /// </summary>
        public Type InterceptorType { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InterceptAttribute"/>.
        /// </summary>
        /// <param name="interceptorType">Тип обработчика.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="interceptorType"/> равен <c>null</c>.</exception>
        public InterceptAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType ?? throw new ArgumentNullException(nameof(interceptorType));
        }
    }
}