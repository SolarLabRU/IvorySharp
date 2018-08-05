using System;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Токен для передачи между сгенерированным прокси и методом перехвата вызовов.
    /// </summary>
    internal struct MethodToken
    {
        /// <summary>
        /// Ключ.
        /// </summary>
        public readonly int Key;
        
        /// <summary>
        /// Объявленный тип.
        /// </summary>
        public readonly Type DeclaringType;

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodToken"/>.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="declaringType">Объявленный тип.</param>
        public MethodToken(int key, Type declaringType)
        {
            Key = key;
            DeclaringType = declaringType;
        }
    }
}