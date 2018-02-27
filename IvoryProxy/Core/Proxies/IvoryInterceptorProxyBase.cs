﻿using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Core.Interceptors;
using IvoryProxy.Core.Providers;
using IvoryProxy.Extensions;

namespace IvoryProxy.Core.Proxies
{
    /// <summary>
    /// Базовый класс прокси, выполняющего перехват методов.
    /// </summary>
    /// <typeparam name="T">Тип класса для проксирования.</typeparam>
    internal class IvoryInterceptorProxyBase<T> : IIvoryProxy<T> where T : class
    {
        /// <inheritdoc />
        public T Decorated { get; }

        /// <inheritdoc />
        public T TransparentProxy { get; }

        /// <summary>
        /// Провайдер перехватчиков.
        /// </summary>
        public IInterceptorProvider InterceptorProvider { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvoryInterceptorProxyBase{T}"/>.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        /// <param name="transparentProxy">Проксированный объект.</param>
        /// <param name="interceptorProvider">Провайдер перехватчиков вызовов.</param>
        internal IvoryInterceptorProxyBase(T decorated, T transparentProxy, IInterceptorProvider interceptorProvider)
        {
            Decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            TransparentProxy = transparentProxy ?? throw new ArgumentNullException(nameof(transparentProxy));
            InterceptorProvider = interceptorProvider ?? throw new ArgumentNullException(nameof(interceptorProvider));
        }

        /// <summary>
        /// Выполняет обработку вызова метода <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        public void Proxy(IInvocation invocation)
        {
            if (invocation.IsInterceptionDisallowed())
            {
                Bypass(invocation);
            }
            else
            {
                var interceptor = InterceptorProvider.GetInterceptor(invocation);
                Intercept(invocation, interceptor);
            }
        }

        /// <summary>
        /// Выполняет вызов метода без изменений.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        protected virtual void Bypass(IInvocation invocation)
        {
            invocation.Proceed();
        }

        /// <summary>
        /// Выполняет перехват вызова метода.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <param name="interceptor">Экземпляр перехватчика вызова.</param>
        protected virtual void Intercept(IInvocation invocation, IInterceptor interceptor)
        {
            try
            {
                interceptor.Intercept(invocation);

                if (invocation is Invocation mi && !mi.IsVoidResult() && !mi.ReturnValueWasSet)
                {
                    throw new IvoryProxyException(
                        $"Не было установлено значение проксированного метода '{invocation.TargetMethod.Name}' " +
                        $"интерфейса '{invocation.DeclaringType.FullName}'. " +
                        $"Проверьте, что в методе '{nameof(IInterceptor.Intercept)}' " +
                        $"перехватчика '{interceptor.GetType().FullName}' устанавливается свойство '{nameof(IInvocation.ReturnValue)}'. " +
                        $"Если полная замена метода не была задумана, то убедитесь в наличии " +
                        $"вызова метода '{nameof(IInvocation.Proceed)}' входного параметра.");
                }
            }
            catch (TargetInvocationException e) when(e.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
        }
    }
}