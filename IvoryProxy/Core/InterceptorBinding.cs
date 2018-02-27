using System;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Пара, отражающая связь между атрибутом и, соответствующим ему,перехватчиком.
    /// </summary>
    internal struct InterceptorBinding
    {
        /// <summary>
        /// Атрибут, отражающий необходимость перехвата метода.
        /// </summary>
        public Type AttributeType;

        /// <summary>
        /// Экземпляр перехватчика
        /// </summary>
        public Type InterceptorType;

        /// <summary>
        /// Место размещения атрибута <see cref="Attribute"/>.
        /// </summary>
        public InterceptorAttributePlacement AttributePlacement;

        /// <summary>
        /// Инициализирует экземпляр <see cref="InterceptorBinding"/>.
        /// </summary>
        /// <param name="attributeType">Тип атрибута.</param>
        /// <param name="interceptorType">Тип перехватчика.</param>
        /// <param name="attributePlacement">Место размещения атрибута.</param>
        public InterceptorBinding(Type attributeType, Type interceptorType, InterceptorAttributePlacement attributePlacement)
        {
            AttributeType = attributeType;
            InterceptorType = interceptorType;
            AttributePlacement = attributePlacement;
        }
    }
}