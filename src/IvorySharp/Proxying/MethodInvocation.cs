using System;
using System.ComponentModel;
using System.Reflection;
using IvorySharp.Linq;
using JetBrains.Annotations;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Модель вызова проксированного метода.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct MethodInvocation
    {
        /// <summary>
        /// Проксированный метод.
        /// </summary>
        public readonly MethodInfo Method;
        
        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        public readonly MethodCall MethodCall;
        
        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        public readonly object TransparentProxy;
        
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        public readonly object[] Arguments;
        
        /// <summary>
        /// Обобшенные параметры.
        /// </summary>
        public readonly Type[] GenericParameters;

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodInvocation"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodCall"></param>
        /// <param name="arguments"></param>
        /// <param name="genericParameters"></param>
        /// <param name="transparentProxy"></param>
        public MethodInvocation(
            MethodInfo method, 
            MethodCall methodCall,
            object[] arguments, 
            Type[] genericParameters, 
            object transparentProxy)
        {
            Method = method;
            MethodCall = methodCall;
            Arguments = arguments ?? Array.Empty<object>();
            GenericParameters = genericParameters ?? Array.Empty<Type>();
            TransparentProxy = transparentProxy;
        }
    }
}