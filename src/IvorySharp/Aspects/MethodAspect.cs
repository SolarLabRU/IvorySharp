using System.Collections.Generic;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый класс для аспектов, применяемых на уровне метода.
    /// </summary>
    [PublicAPI]
    public abstract class MethodAspect : AspectAttribute
    {
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Выполняет инициализацию аспекта.
        /// </summary>
        //  ReSharper disable once VirtualMemberNeverOverridden.Global (PublicAPI)
        public virtual void Initialize() { }

        /// <summary>
        /// Точка прикрепления аспекта.
        /// </summary>
        public MethodAspectJoinPointType JoinPointType { get; internal set; }

        /// <summary>
        /// Сравнение аспектов на основе типов.
        /// </summary>
        internal class ByTypeEqualityComparer : EqualityComparer<MethodAspect>
        {
            public static readonly ByTypeEqualityComparer Instance = new ByTypeEqualityComparer();

            private ByTypeEqualityComparer() { }

            /// <inheritdoc />
            public override bool Equals(MethodAspect x, MethodAspect y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.GetType() == y.GetType();
            }

            /// <inheritdoc />
            public override int GetHashCode(MethodAspect obj)
            {
                return obj == null ? 0 : obj.GetType().GetHashCode();
            }
        }
    }
}