
namespace IvorySharp.Proxying.Platform.NetFramework
{
    #if NET461

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using Aspects.Weaving;
    using Core;
    using Extensions;

    /// <summary>
    /// Реализация прокси на основе <see cref="RealProxy"/>.
    /// </summary>
    public class InterceptRealProxy : RealProxy
    {
        private readonly IInterceptor _interceptor;
        private readonly object _target;
        private readonly Type _targetDeclaredType;

        /// <summary>
        /// Инициализирует экземпляр прокси <see cref="InterceptRealProxy"/>.
        /// </summary>
        /// <param name="target">Экземпляр объекта для создания прокси.</param>
        /// <param name="targetDeclaredType">Тип прокси. Задается как интерфейс, который реализуется типом объекта <paramref name="target"/>.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        internal InterceptRealProxy(object target, Type targetDeclaredType, IInterceptor interceptor)
            : base(targetDeclaredType)
        {
            _targetDeclaredType = targetDeclaredType;
            _interceptor = interceptor;
            _target = target;
        }

        /// <summary>
        /// Создает экземпляр прокси объекта типа <paramref name="targetDeclaredType"/>.
        /// При этом тип обязательно должен быть интерфейсом.
        /// Все обращения к методам объекта <paramref name="target"/>, которые реализуются от интерфейса
        /// <paramref name="targetDeclaredType"/> будут проксированы через метод <see cref="Invoke(IMessage)"/>
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
            
            var proxy = new InterceptRealProxy(target, targetDeclaredType, interceptor);
            return proxy.GetTransparentProxy();
        }

        /// <inheritdoc />
        public override IMessage Invoke(IMessage msg)
        {
            if (msg is IMethodCallMessage mcm && mcm.MethodBase is MethodInfo mi)
            {
                try
                {
                    return (AspectWeaver.NotInterceptableMethods.Any(m => ReferenceEquals(m, mi)))
                        ? Bypass(mcm)
                        : Intercept(mcm);
                }
                catch (TargetInvocationException e)
                {
                    e.Unwrap().Rethrow();
                }
            }

            return msg;
        }

        /// <summary>
        /// Вызов базового метода без перехвата.
        /// </summary>
        private IMessage Bypass(IMethodCallMessage message)
        {
            var value = message.MethodBase.Invoke(_target, message.Args);
            return new ReturnMessage(value, message.Args, message.Args.Length, null, message);
        }

        /// <summary>
        /// Выполнение проксирования метода.
        /// </summary>
        private IMessage Intercept(IMethodCallMessage message)
        {
            var ctx = new InvocationContext(message.Args, (MethodInfo)message.MethodBase, _target, _targetDeclaredType);
            var invocation = new Invocation(ctx);

            _interceptor.Intercept(invocation);

            var returnValue = invocation.Context.Method.IsVoidReturn()
                ? default(object)
                : invocation.Context.ReturnValue;

            return new ReturnMessage(
                returnValue, 
                (object[])invocation.Context.Arguments, 
                invocation.Context.Arguments.Count,
                null, 
                message);
        }
    }

    #endif
}
