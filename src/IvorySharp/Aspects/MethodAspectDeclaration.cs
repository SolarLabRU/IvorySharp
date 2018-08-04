using System;
using System.Collections.Generic;
using System.Reflection;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Объявление аспекта.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    public class MethodAspectDeclaration<TAspect> where TAspect : MethodAspect
    {
        /// <summary>
        /// Атрибут аспекта.
        /// </summary>
        public TAspect MethodAspect { get; }

        /// <summary>
        /// Тип точки прикрепления аспекта.
        /// </summary>
        public MethodAspectJoinPointType JoinPointType { get; }

        /// <summary>
        /// Метод, либо тип на котором закреплен аспект.
        /// </summary>
        public MemberInfo AspectDeclaredMember { get; }

        private MethodAspectDeclaration(TAspect methodAspect, MethodAspectJoinPointType joinPointType, MemberInfo aspectDeclaredMember)
        {
            MethodAspect = methodAspect;
            JoinPointType = joinPointType;
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
            return new MethodAspectDeclaration<TAspect>(aspect, MethodAspectJoinPointType.Type, type);
        }

        /// <summary>
        /// Создает декларацию аспекта из метода.
        /// </summary>
        /// <param name="aspect">Тип аспекта.</param>
        /// <param name="method">Метод.</param>
        /// <returns>Декларация аспекта.</returns>
        public static MethodAspectDeclaration<TAspect> FromMethod(TAspect aspect, MethodInfo method)
        {
            return new MethodAspectDeclaration<TAspect>(aspect, MethodAspectJoinPointType.Method, method);
        }

        /// <summary>
        /// Сравнивает две декларации аспектов на основе типа аспекта.
        /// </summary>
        internal class ByAspectTypeEqualityComparer : EqualityComparer<MethodAspectDeclaration<TAspect>>
        {
            /// <summary>
            /// Инициализированный экземпляр <see cref="ByAspectTypeEqualityComparer"/>.
            /// </summary>
            public static readonly ByAspectTypeEqualityComparer Instance = new ByAspectTypeEqualityComparer();

            private ByAspectTypeEqualityComparer() { }

            /// <inheritdoc />
            public override bool Equals(MethodAspectDeclaration<TAspect> x, MethodAspectDeclaration<TAspect> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;

                return Aspects.MethodAspect.ByTypeEqualityComparer.Instance.Equals(x.MethodAspect, y.MethodAspect);
            }

            /// <inheritdoc />
            public override int GetHashCode(MethodAspectDeclaration<TAspect> obj)
            {
                if (obj == null)
                    return 0;
                return Aspects.MethodAspect.ByTypeEqualityComparer.Instance.GetHashCode(obj.MethodAspect);
            }
        }
    }
}