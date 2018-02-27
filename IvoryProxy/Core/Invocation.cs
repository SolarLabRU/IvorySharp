using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using IvoryProxy.Helpers;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Модель вызова метода.
    /// </summary>
    internal class Invocation : IInvocation
    {
        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <summary>
        /// Экземпляр целевого объекта, метод которого был вызван.
        /// </summary>
        public object InvocationTarget { get; }

        /// <summary>
        /// Массив параметров, с которыми был вызван метод.
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        public object ReturnValue
        {
            get => _returnValue;
            set { 
                ReturnValueWasSet = true;
                _returnValue = value;
            }
        }

        private object _returnValue;

        /// <summary>
        /// Признак того, что возвращаемое значение было установлено.
        /// </summary>
        public bool ReturnValueWasSet { get; set; }

        /// <summary>
        /// Метод, вызов которого был запрошен.
        /// </summary>
        public MethodInfo TargetMethod { get; }
    
        /// <summary>
        /// Инициализирует экземпляр <see cref="Invocation"/>.
        /// </summary>
        /// <param name="target">Экземпляр целевого объекта, метод которого был вызван.</param>
        /// <param name="arguments">Массив параметров, с которыми был вызван метод.</param>
        /// <param name="targetMethod">Метод, вызов которого был запрошен.</param>
        /// <param name="declaringType">Тип, в котором определен метод <paramref name="targetMethod"/>.</param>
        public Invocation(object target, object[] arguments, MethodInfo targetMethod, Type declaringType)
        {
            InvocationTarget = target;
            Arguments = arguments ?? Array.Empty<object>();
            TargetMethod = targetMethod;
            DeclaringType = declaringType;
        }

        /// <inheritdoc />
        public void Proceed()
        {
            try
            {
                ReturnValue = TargetMethod.Invoke(InvocationTarget, Arguments);
                ReturnValueWasSet = true;
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(ExceptionHelper.UnwrapTargetInvocationException(e)).Throw();
            }
        }
    }
}