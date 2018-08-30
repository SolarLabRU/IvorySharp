using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        /// Признак наличия зависимостей у аспекта.
        /// </summary>
        internal bool HasDependencies { get; set; }
        
        /// <summary>
        /// Признак того, что аспект поддерживает финализацию.
        /// </summary>
        internal bool IsFinalizable { get; set; }
        
        /// <summary>
        /// Признак того, что аспект поддерживает инициализацию.
        /// </summary>
        internal bool IsInitializable { get; set; }
        
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

        /// <summary>
        /// Возвращает ссылку на метод <see cref="Initialize()"/>.
        /// </summary>
        /// <param name="aspect">Аспект.</param>
        /// <returns>Ссылка на метод <see cref="Initialize()"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static MethodInfo GetInitializeMethod([NotNull] MethodAspect aspect)
        {
            return aspect.GetType().GetMethod(nameof(Initialize));
        }
        
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