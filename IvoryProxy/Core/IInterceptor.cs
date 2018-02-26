﻿namespace IvoryProxy.Core
{
    /// <summary>
    /// Компонент, выполняющий пехеват вызова методов целевого объекта.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Выполняет перехват вызова метода <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        void Intercept(IMethodInvocation invocation);

        /// <summary>
        /// Определяет возможность перехвата вызова метода.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Признак invocation перехвата вызова метода.</returns>
        bool CanIntercept(IMethodInvocation invocation);
    }
}