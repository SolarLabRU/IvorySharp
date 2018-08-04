using System;
using IvorySharp.Core;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Интерфейс генератора прокси для перехватов вызова методов объекта.
    /// </summary>
    internal interface IInterceptProxyGenerator
    {
        /// <summary>
        /// Создает экземпляр прокси интерфейса с типом <typeparamref name="TInterface"/> от объекта
        /// <paramref name="target"/>. При этом все вызовы к исходным методам объекта проксируются через
        /// перехватчик <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта.</param>
        /// <param name="interceptor">Экземпляр перехватчика.</param>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TClass"></typeparam>
        /// <returns>Экземпляр прокси.</returns>
        TInterface CreateInterceptProxy<TInterface, TClass>(TClass target, IInterceptor interceptor)
            where TInterface : class
            where TClass : TInterface;

        /// <summary>
        /// Создает экземпляр прокси интерфейса с типом <paramref name="declaredType"/> от объекта
        /// <paramref name="target"/>. При этом все вызовы к исходным методам объекта проксируются через
        /// перехватчик <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта.</param>
        /// <param name="declaredType">Объявленный тип (интерфейса).</param>
        /// <param name="targetType">Тип класса, реализующего интерфейс <paramref name="declaredType"/>.</param>
        /// <param name="interceptor">Экземпляр перехватчика.</param>
        /// <returns>Экземпляр прокси.</returns>
        object CreateInterceptProxy(object target, Type declaredType, Type targetType, IInterceptor interceptor);
    }
}