using System;
using System.Reflection;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Модель вызова метода.
    /// </summary>
    internal class MethodInvocation : IMethodInvocation, IMethodPreExecutionContext, IMethodPostExecutionContext
    {
        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <summary>
        /// Экземпляр целевого объекта, метод которого был вызван.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Массив параметров, с которыми был вызван метод.
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        public object ReturnValue { get; private set; }

        /// <summary>
        /// Признак того, что метод возвращает <see cref="System.Void"/>.
        /// </summary>
        public bool IsReturnVoid { get; }

        /// <summary>
        /// Метод, вызов которого был запрошен.
        /// </summary>
        public MethodInfo TargetMethod { get; }
    
        /// <summary>
        /// Признак того, что возвращаемое значение было установлено.
        /// </summary>
        public bool IsReturnValueWasSet { get; private set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodInvocation"/>.
        /// </summary>
        /// <param name="target">Экземпляр целевого объекта, метод которого был вызван.</param>
        /// <param name="arguments">Массив параметров, с которыми был вызван метод.</param>
        /// <param name="targetMethod">Метод, вызов которого был запрошен.</param>
        /// <param name="declaringType">Тип, в котором определен метод <paramref name="targetMethod"/>.</param>
        public MethodInvocation(object target, object[] arguments, MethodInfo targetMethod, Type declaringType)
        {
            Target = target;
            Arguments = arguments ?? Array.Empty<object>();
            TargetMethod = targetMethod;
            DeclaringType = declaringType;
            IsReturnVoid = targetMethod.ReturnType == typeof(void);
        }

        /// <inheritdoc />
        public bool TrySetReturnValue(object result)
        {
            if (IsReturnVoid)
            {
                return false;
            }

            IsReturnValueWasSet = true;
            ReturnValue = result;
            return true;
        }

        /// <inheritdoc />
        public void Proceed()
        {
            var result = TargetMethod.Invoke(Target, Arguments);
            TrySetReturnValue(result);
        }
    }
}