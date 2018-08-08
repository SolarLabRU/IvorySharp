using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Comparers;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Параметры вызова метода.
    /// </summary>
    [PublicAPI]
    public class InvocationContext
    {        
        /// <summary>
        /// Возвращаемое значение метода.
        /// </summary>
        [PublicAPI] public object ReturnValue { get; internal set; }
        
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        [PublicAPI] public IReadOnlyCollection<object> Arguments { get; }
        
        /// <summary>
        /// Вызываемый метод.
        /// </summary>
        [PublicAPI] public MethodInfo Method { get; }
        
        /// <summary>
        /// Экземпляр класса, в котором содержится вызываемый метод.
        /// </summary>
        [PublicAPI] public object Instance { get; }
        
        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        [PublicAPI] public object TransparentProxy { get; }

        /// <summary>
        /// Тип в котором определен вызываемый метод (интерфейс).
        /// </summary>
        [PublicAPI] public Type DeclaringType { get; }
        
        /// <summary>
        /// Тип в котором содержится реализация вызываемого метода.
        /// </summary>
        [PublicAPI] public Type TargetType { get; }

        /// <summary>
        /// Инициализирует экземпляр модели вызова метода.
        /// </summary>
        /// <param name="arguments">Параметры вызова.</param>
        /// <param name="method">Вызываемый метод.</param>
        /// <param name="instance">Экземпляр объекта.</param>
        /// <param name="transparentProxy">Прокси.</param>
        /// <param name="declaringType">Тип в котором определен вызываемый метод (интерфейс).</param>
        /// <param name="targetType">Тип в котором содержится реализация вызываемого метода.</param>
        internal InvocationContext(
            IReadOnlyCollection<object> arguments, 
            MethodInfo method, 
            object instance, 
            object transparentProxy, 
            Type declaringType, 
            Type targetType)
        {
            Arguments = arguments;
            Method = method;
            Instance = instance;
            DeclaringType = declaringType;
            TargetType = targetType;
            TransparentProxy = transparentProxy;
        }      
        
        /// <summary>
        /// Выполняет сравнение контекстов на основе метода.
        /// </summary>
        internal sealed class ByMethodEqualityComparer : IEqualityComparer<InvocationContext>
        {
            /// <summary>
            /// Инициализированный экземпляр <see cref="ByMethodEqualityComparer"/>.
            /// </summary>
            public static readonly ByMethodEqualityComparer Instance = new ByMethodEqualityComparer();
            
            /// <inheritdoc />
            public bool Equals(InvocationContext x, InvocationContext y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return MethodEqualityComparer.Instance.Equals(x.Method, y.Method);
            }

            /// <inheritdoc />
            public int GetHashCode(InvocationContext obj)
            {
                return obj.Method.GetHashCode();
            }
        }
    }
}