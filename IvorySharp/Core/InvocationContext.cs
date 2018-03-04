using System;
using System.Collections.Generic;
using System.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Параметры вызова метода.
    /// </summary>
    public class InvocationContext
    {
        /// <summary>
        /// Возвращаемое значение метода.
        /// </summary>
        public object ReturnValue { get; set; }
        
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        public IReadOnlyCollection<object> Arguments { get; }
        
        /// <summary>
        /// Вызываемый метод.
        /// </summary>
        public MethodInfo Method { get; }
        
        /// <summary>
        /// Сущность, метод который был перехвачен.
        /// </summary>
        public object Instance { get; }
        
        /// <summary>
        /// Объявленный тип сущности, метод которой был вызван.
        /// Может быть интерфейсом, который реализуется экземпляром <see cref="Instance"/>.
        /// </summary>
        public Type InstanceDeclaringType { get; }
        
        /// <summary>
        /// Инициализирует экземпляр модели вызова метода.
        /// </summary>
        /// <param name="arguments">Параметры вызова.</param>
        /// <param name="method">Вызываемый метод.</param>
        /// <param name="instance">Экземпляр объекта.</param>
        /// <param name="instanceDeclaringType">Объявленный тип объекта.</param>
        public InvocationContext(
            IReadOnlyCollection<object> arguments, 
            MethodInfo method, 
            object instance, 
            Type instanceDeclaringType)
        {
            Arguments = arguments;
            Method = method;
            Instance = instance;
            InstanceDeclaringType = instanceDeclaringType;
        }
    }
}