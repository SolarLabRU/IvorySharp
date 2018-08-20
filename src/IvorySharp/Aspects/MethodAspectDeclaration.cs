using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Объявление аспекта.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class MethodAspectDeclaration<TAspect> where TAspect : MethodAspect
    {
        /// <summary>
        /// Атрибут аспекта.
        /// </summary>
        public TAspect MethodAspect { get; }

        /// <summary>
        /// Тип элемента, к которому прикрелпен аспект.
        /// </summary>
        public MethodAspectMulticastTarget MulticastTarget { get; }

        /// <summary>
        /// Метод, либо тип на котором закреплен аспект.
        /// </summary>
        public MemberInfo AspectDeclaredMember { get; }

        private MethodAspectDeclaration(TAspect methodAspect, MethodAspectMulticastTarget multicastTarget, MemberInfo aspectDeclaredMember)
        {
            MethodAspect = methodAspect;
            MulticastTarget = multicastTarget;
            AspectDeclaredMember = aspectDeclaredMember;
        }

        /// <summary>
        /// Создает декларацию аспекта из типа.
        /// </summary>
        /// <param name="aspect">Тип аспекта.</param>
        /// <param name="type">Тип.</param>
        /// <returns>Декларация аспекта.</returns>
        public static MethodAspectDeclaration<TAspect> FromType(TAspect aspect, Type type)
        {
            aspect.MulticastTarget = MethodAspectMulticastTarget.Type;
            return new MethodAspectDeclaration<TAspect>(aspect, MethodAspectMulticastTarget.Type, type);
        }

        /// <summary>
        /// Создает декларацию аспекта из метода.
        /// </summary>
        /// <param name="aspect">Тип аспекта.</param>
        /// <param name="method">Метод.</param>
        /// <returns>Декларация аспекта.</returns>
        public static MethodAspectDeclaration<TAspect> FromMethod(TAspect aspect, MethodInfo method)
        {
            aspect.MulticastTarget = MethodAspectMulticastTarget.Method;
            return new MethodAspectDeclaration<TAspect>(aspect, MethodAspectMulticastTarget.Method, method);
        }
    }
}