using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Сигнатура вызова (доступна до фактического вызова метода).
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInvocationSignature
    {
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
        /// Тип вызываемого метода.
        /// </summary>
        InvocationType InvocationType { get; }
    }
}