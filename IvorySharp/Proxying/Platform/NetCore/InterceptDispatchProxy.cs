using System;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Proxying.Platform.NetCore
{
    /// <summary>
    /// Прокси для перехвата методов на основе <see cref="DispatchProxy"/>.
    /// </summary>
    public class InterceptDispatchProxy : DispatchProxy
    {
        private IInterceptor _interceptor;
        private object _target;
        private Type _targetDeclaredType;
        
        /// <summary>
        /// Создает экземпляр прокси объекта типа <paramref name="targetDeclaredType"/>.
        /// При этом тип обязательно должен быть интерфейсом.
        /// Все обращения к методам объекта <paramref name="target"/>, которые реализуются от интерфейса
        /// <paramref name="targetDeclaredType"/> будут проксированы через метод <see cref="Invoke(MethodInfo, object[])"/>
        /// и перехвачены обработчиком <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта для создания прокси.</param>
        /// <param name="targetDeclaredType">Тип прокси. Задается как интерфейс, который реализуется типом объекта <paramref name="target"/>.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        /// <returns>Экземляр прокси типа <paramref name="targetDeclaredType"/>.</returns>
        public static object CreateTransparentProxy(object target, Type targetDeclaredType, IInterceptor interceptor)
        {
            if (!targetDeclaredType.IsInterface)
            {
                throw new InvalidOperationException(
                    "Проксирование допускается только для интерфейсов. " +
                    $"Параметр '{nameof(targetDeclaredType)}': {targetDeclaredType.FullName}");
            }
            
            var transparentProxy = CreateTrasparentProxy<InterceptDispatchProxy>(targetDeclaredType);
            var interceptProxy = (InterceptDispatchProxy) transparentProxy;
            
            interceptProxy.Initialize(target, targetDeclaredType, interceptor);

            return transparentProxy;
        }

        /// <inheritdoc />
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                var invocation = new Invocation(new InvocationContext(args, targetMethod, _target, _targetDeclaredType));
                _interceptor.Intercept(invocation);
                
                if (!targetMethod.IsVoidReturn())
                    return invocation.Context.ReturnValue;
            }
            catch (TargetInvocationException e)
            {
                e.Unwrap().Rethrow();
            }

            return default(object);
        }

        /// <summary>
        /// Выполняет инициализацию прокси.
        /// </summary>
        /// <param name="target">Экземпляр объекта для создания прокси.</param>
        /// <param name="targetDeclaredType">Тип прокси. Задается как интерфейс, который реализуется типом объекта <paramref name="target"/>.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        private void Initialize(object target, Type targetDeclaredType, IInterceptor interceptor)
        {
            _target = target;
            _targetDeclaredType = targetDeclaredType;
            _interceptor = interceptor;
        }
    }
}