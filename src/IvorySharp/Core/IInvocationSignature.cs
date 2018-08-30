using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Сигнатура вызова (доступна до фактического вызова метода).
    /// </summary>
    [PublicAPI]
    public interface IInvocationSignature
    {
        /// <summary>
        /// Метод, вызов которого перехватываается.
        /// Обычно это метод, находящийся в интерфейсе, который реализуется
        /// целевым классом.
        /// </summary>
        [NotNull] MethodInfo Method { get; }
        
        /// <summary>
        /// Метод, находящийся в фактическом типе целевого объекта (классе).
        /// </summary>
        [NotNull] MethodInfo TargetMethod { get; }
        
        /// <summary>
        /// Объявленный тип целевого объекта (интерфейс).
        /// </summary>
        [NotNull] Type DeclaringType { get; }

        /// <summary>
        /// Фактический тип целевого объекта (класс).
        /// </summary>
        [NotNull] Type TargetType { get; }
                    
        /// <summary>
        /// Тип вызываемого метода.
        /// </summary>
        InvocationType InvocationType { get; }
    }
}