using System;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Activators
{
    /// <summary>
    /// Активатор перехватчика вызова метода.
    /// </summary>
    public interface IInterceptorActivator
    {
        /// <summary>
        /// Создает экземпляр перехватчика вызова метода по типу.
        /// </summary>
        /// <param name="interceptorType">Тип перехватчика.</param>
        /// <returns>Экземпляр перехватчика.</returns>
        IInterceptor CreateInstance(Type interceptorType);
    }
}