using System;
using System.ComponentModel;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IvorySharp.Proxying;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Прокси, связанное с аспектами.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public class AspectWeaveProxy : IvoryProxy
    {
        /// <summary>
        /// Исходный объект, вызовы которого будут перехватываться.
        /// </summary>
        internal object Target { get; private set; }

        /// <summary>
        /// Тип, в котором объявлен целевой метод (интерфейс).
        /// </summary>
        internal Type DeclaringType { get; private set; }

        /// <summary>
        /// Тип, в котором содержится реализация целевого метода.
        /// </summary>
        internal Type TargetType { get; private set; }
      
        /// <summary>
        /// Кеш информации о методах
        /// </summary>
        internal IMethodInfoCache MethodCache { get; private set; }
        
        /// <summary>
        /// Компонент для перехвата вызова метода.
        /// </summary>
        internal InvocationInterceptor Interceptor { get; private set; }

        /// <summary>
        /// Создает экземпляр прокси.
        /// </summary>
        /// <param name="target">Целевой объект</param>
        /// <param name="targetType">Тип целевого объекта.</param>
        /// <param name="declaringType">Тип интерфейса, реализуемого целевым классом.</param>
        /// <param name="invocationWeaveDataProvider">Провайдер данных о вызове.</param>
        /// <param name="aspectDependencyInjectorHolder">Компонент для внедрения зависимостей в аспекты.</param>
        /// <param name="aspectFinalizerHolder">Финализатор аспектов.</param>
        /// <param name="methodInfoCache">Кеш методов.</param>
        /// <returns>Экземпляр прокси.</returns>
        internal static object Create(
            object target,
            Type targetType,
            Type declaringType,
            IInvocationWeaveDataProvider invocationWeaveDataProvider,
            IComponentHolder<IAspectDependencyInjector> aspectDependencyInjectorHolder,
            IComponentHolder<IAspectFinalizer> aspectFinalizerHolder,
            IMethodInfoCache methodInfoCache)
        {
            var transparentProxy = ProxyGenerator.Instance.CreateTransparentProxy(
                typeof(AspectWeaveProxy), declaringType);
            
            var weavedProxy = (AspectWeaveProxy) transparentProxy;

            weavedProxy.Initialize(
                target, 
                targetType,
                declaringType,
                invocationWeaveDataProvider,
                aspectDependencyInjectorHolder,
                aspectFinalizerHolder,
                methodInfoCache);

            return transparentProxy;
        }

        /// <summary>
        /// Выполняет инициализацию прокси.
        /// </summary>
        private void Initialize(
            object target,
            Type targetType,
            Type declaringType,
            IInvocationWeaveDataProvider invocationWeaveDataProvider,
            IComponentHolder<IAspectDependencyInjector> aspectDependencyInjectorHolder,
            IComponentHolder<IAspectFinalizer> aspectFinalizerHolder,
            IMethodInfoCache methodInfoCache)    
        {
            Target = target;
            TargetType = targetType;
            DeclaringType = declaringType;
            
            Interceptor = new InvocationInterceptor(
                invocationWeaveDataProvider,
                aspectDependencyInjectorHolder,
                aspectFinalizerHolder);

            MethodCache = methodInfoCache;
        }

        /// <inheritdoc />
        protected internal override object Invoke(MethodInvocation invocation)
        {
            var targetMethod = MethodCache.GetMethodMap(TargetType, invocation.Method);
            var signature = new InvocationSignature(
                invocation.Method, targetMethod, DeclaringType,
                TargetType, invocation.Method.GetInvocationType());

            return Interceptor.Intercept(
                signature, invocation.MethodLambda,
                invocation.Arguments, Target, 
                invocation.TransparentProxy);
        }
    }
}