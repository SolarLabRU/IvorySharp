using System;
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
        /// Идентификатор аспекта.
        /// </summary>
        internal Guid InternalId { get; set; }
        
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
        /// Тип элемента, к которому прикреплен аспект.
        /// </summary>
        public MethodAspectMulticastTarget MulticastTarget { get; internal set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}