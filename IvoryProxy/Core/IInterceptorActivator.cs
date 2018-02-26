using System;

namespace IvoryProxy.Core
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