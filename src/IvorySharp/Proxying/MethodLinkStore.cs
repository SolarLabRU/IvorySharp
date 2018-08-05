using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Хранит связь вида [Method -> Token] [Token -> Method].
    /// </summary>
    internal class MethodLinkStore
    {
        private readonly Dictionary<MethodBase, int> _methodToKey;
        private readonly List<MethodBase> _methodsByKey;

        public MethodLinkStore()
        {
            _methodToKey = new Dictionary<MethodBase, int>();
            _methodsByKey = new List<MethodBase>();
        }

        /// <summary>
        /// Создает токен из метода.
        /// </summary>
        /// <param name="method">Метод.</param>
        /// <returns>Токен.</returns>
        public MethodToken CreateToken(MethodInfo method)
        {
            if (!_methodToKey.TryGetValue(method, out var key))
            {
                _methodsByKey.Add(method);
                key = _methodsByKey.Count - 1;
                _methodToKey[method] = key;
            }

            return new MethodToken(key, method.DeclaringType);
        }

        /// <summary>
        /// Возвращает метод по сгенерированному токену.
        /// </summary>
        /// <param name="methodToken">Токен.</param>
        public MethodBase ResolveMethod(MethodToken methodToken)
        {
            Debug.Assert(methodToken.Key >= 0, "token.Key >= 0");
            Debug.Assert(methodToken.Key < _methodsByKey.Count, "token.Key < _methodsByKey.Count");

            return _methodsByKey[methodToken.Key];
        }
    }
}