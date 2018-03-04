using System;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый атрибут аспекта.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public abstract class AspectAttribute : Attribute
    {
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Порядок атрибута.
        /// </summary>
        public int Order { get; set; }
    }
}