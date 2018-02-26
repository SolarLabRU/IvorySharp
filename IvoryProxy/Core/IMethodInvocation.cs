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
        object Target { get; }
        
        /// <summary>
        /// Массив параметров, с которыми был вызван метод.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        object ReturnValue { get; }
        
        /// <summary>
        /// Признак того, что метод возвращает <see cref="System.Void"/>.
        /// </summary>
        bool IsReturnVoid { get; }
        
        /// <summary>
        /// Признак того, что возвращаемое значение было установлено.
        /// </summary>
        bool IsReturnValueWasSet { get; }

        /// <summary>
        /// Метод, вызов которого был запрошен.
        /// </summary>
        MethodInfo TargetMethod { get; }

        /// <summary>
        /// Выполнение вызова.
        /// </summary>
        /// <returns>Результат вызова.</returns>
        void Proceed();

        /// <summary>
        /// Выполняет попытку установки результата вызова метода.
        /// </summary>
        /// <param name="result">Результат вызова метода.</param>
        bool TrySetReturnValue(object result);
    }
}