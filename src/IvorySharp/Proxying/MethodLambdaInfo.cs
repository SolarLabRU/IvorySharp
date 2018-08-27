using System;
using System.Reflection;
using IvorySharp.Linq;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Расширенная информация о методе.
    /// </summary>
    internal readonly struct MethodLambdaInfo : IEquatable<MethodLambdaInfo>
    {
        /// <summary>
        /// Делегат для быстрого вызова.
        /// </summary>
        public readonly MethodLambda MethodLambda;
            
        /// <summary>
        /// Исходный метод.
        /// </summary>
        public readonly MethodInfo MethodInfo;

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodLambdaInfo"/>.
        /// </summary>
        /// <param name="methodInfo">Оригинальный метод.</param>
        /// <param name="methodLambda">Делегат для быстрого вызова.</param>
        public MethodLambdaInfo(MethodInfo methodInfo, MethodLambda methodLambda)
        {
            MethodInfo = methodInfo;
            MethodLambda = methodLambda;
        }
            
        /// <inheritdoc />
        public bool Equals(MethodLambdaInfo other)
        {
            return MethodInfo.Equals(other.MethodInfo);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MethodLambdaInfo info && Equals(info);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }
    }
}