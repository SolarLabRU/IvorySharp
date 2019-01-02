using System;
using System.Reflection;
using IvorySharp.Linq;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Расширенная информация о методе.
    /// </summary>
    internal readonly struct ExtendedMethodInfo : IEquatable<ExtendedMethodInfo>
    {
        /// <summary>
        /// Делегат для быстрого вызова.
        /// </summary>
        public readonly MethodCall MethodCall;
            
        /// <summary>
        /// Исходный метод.
        /// </summary>
        public readonly MethodInfo MethodInfo;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ExtendedMethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">Оригинальный метод.</param>
        /// <param name="methodCall">Делегат для быстрого вызова.</param>
        public ExtendedMethodInfo(MethodInfo methodInfo, MethodCall methodCall)
        {
            MethodInfo = methodInfo;
            MethodCall = methodCall;
        }
            
        /// <inheritdoc />
        public bool Equals(ExtendedMethodInfo other)
        {
            return MethodInfo.Equals(other.MethodInfo);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ExtendedMethodInfo info && Equals(info);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }
    }
}