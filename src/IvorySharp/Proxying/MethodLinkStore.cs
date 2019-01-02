using System.Collections.Generic;
using System.Diagnostics;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Хранит связь вида [Method -> Token] [Token -> Method].
    /// </summary>
    internal sealed class MethodLinkStore
    {        
        private readonly Dictionary<ExtendedMethodInfo, int> _methodToKey;
        private readonly List<ExtendedMethodInfo> _methodsByKey;

        public MethodLinkStore()
        {
            _methodToKey = new Dictionary<ExtendedMethodInfo, int>();
            _methodsByKey = new List<ExtendedMethodInfo>();
        }

        /// <summary>
        /// Создает токен из метода.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Токен.</returns>
        public MethodToken CreateToken(ExtendedMethodInfo method)
        {
            if (_methodToKey.TryGetValue(method, out var key))
                return new MethodToken(key, method.MethodInfo.DeclaringType);
            
            _methodsByKey.Add(method);
            key = _methodsByKey.Count - 1;
            _methodToKey[method] = key;

            return new MethodToken(key, method.MethodInfo.DeclaringType);
        }

        /// <summary>
        /// Возвращает метод по сгенерированному токену.
        /// </summary>
        /// <param name="methodToken">Токен.</param>
        public ExtendedMethodInfo ResolveMethod(MethodToken methodToken)
        {
            Debug.Assert(methodToken.Key >= 0, "token.Key >= 0");
            Debug.Assert(methodToken.Key < _methodsByKey.Count, "token.Key < _methodsByKey.Count");

            return _methodsByKey[methodToken.Key];
        }
    }
}