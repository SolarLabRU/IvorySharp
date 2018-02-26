using System;
using System.Reflection;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Интерфейс модели вызова метода.
    /// </summary>
    public interface IMethodInvocation
    {
        /// <summary>
        /// Тип, в котором определен метод <see cref="TargetMethod"/>.
        /// </summary>
        Type DeclaringType { get; }
            
        /// <summary>
        /// Экземпляр целевого объекта, метод которого был вызван.
        /// </summary>
        object InvocationTarget { get; }
        
        /// <summary>
        /// Массив параметров, с которыми был вызван метод.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        object ReturnValue { get; set; }    

        /// <summary>
        /// Метод, вызов которого был запрошен.
        /// </summary>
        MethodInfo TargetMethod { get; }

        /// <summary>
        /// Выполнение вызова.
        /// </summary>
        /// <returns>Результат вызова.</returns>
        void Proceed();
    }
}