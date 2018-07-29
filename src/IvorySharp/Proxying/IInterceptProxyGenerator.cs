using System;
using IvorySharp.Core;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Интерфейс генератора прокси для перехватов вызова методов объекта.
    /// </summary>
    public interface IInterceptProxyGenerator
    {
        /// <summary>
        /// Создает экземпляр прокси интерфейса с типом <typeparamref name="T"/> от объекта
        /// <paramref name="target"/>. При этом все вызовы к исходным методам объекта проксируются через
        /// перехватчик <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта.</param>
        /// <param name="interceptor">Экземпляр перехватчика.</param>
        /// <typeparam name="T">Тип интерфейса, который реализует экземпляр для проксирования.</typeparam>
        /// <returns>Экземпляр прокси.</returns>
        T CreateInterceptProxy<T>(T target, IInterceptor interceptor) where  T : class;

        /// <summary>
        /// Создает экземпляр прокси интерфейса с типом <paramref name="targetDeclaredType"/> от объекта
        /// <paramref name="target"/>. При этом все вызовы к исходным методам объекта проксируются через
        /// перехватчик <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта.</param>
        /// <param name="targetDeclaredType">Объявленный тип объекта (такого же типа будет прокси).</param>
        /// <param name="interceptor">Экземпляр перехватчика.</param>
        /// <returns>Экземпляр прокси.</returns>
        object CreateInterceptProxy(object target, Type targetDeclaredType, IInterceptor interceptor);
    }
}