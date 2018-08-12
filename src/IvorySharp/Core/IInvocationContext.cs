using System;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Контекст вызова метода.
    /// </summary>
    [PublicAPI]
    public interface IInvocationContext
    {
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        InvocationArguments Arguments { get; }
        
        /// <summary>
        /// Метод, вызов которого перехватываается.
        /// Обычно это метод, находящийся в интерфейсе, который реализуется
        /// целевым классом.
        /// </summary>
        MethodInfo Method { get; }
        
        /// <summary>
        /// Метод, находящийся в фактическом типе целевого объекта (классе).
        /// </summary>
        MethodInfo TargetMethod { get; }
        
        /// <summary>
        /// Объявленный тип целевого объекта (интерфейс).
        /// </summary>
        Type DeclaringType { get; }

        /// <summary>
        /// Фактический тип целевого объекта (класс).
        /// </summary>
        Type TargetType { get; }
        
        /// <summary>
        /// Прокси целевого объекта.
        /// </summary>
        object Proxy { get; }
        
        /// <summary>
        /// Экземпляр целевого объекта.
        /// </summary>
        object Target { get; }
        
        /// <summary>
        /// Тип вызываемого метода.
        /// </summary>
        InvocationType InvocationType { get; }
    }
}