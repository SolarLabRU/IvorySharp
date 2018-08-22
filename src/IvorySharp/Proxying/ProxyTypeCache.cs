using System;
using System.Collections.Generic;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Кеш сгенерированных прокси типов.
    /// </summary>
    internal sealed class ProxyTypeCache
    {
        private readonly Dictionary<Type, Dictionary<Type, Type>> _internalCache;
        private readonly ProxyTypeGeneratorFacade _proxyTypeGeneratorFacade;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ProxyTypeCache"/>.
        /// </summary>
        public ProxyTypeCache(ProxyTypeGeneratorFacade proxyTypeGeneratorFacade)
        {
            _proxyTypeGeneratorFacade = proxyTypeGeneratorFacade;
            _internalCache = new Dictionary<Type, Dictionary<Type, Type>>();
        }

        /// <summary>
        /// Получает тип прокси.
        /// </summary>
        /// <param name="baseProxyType">Базовый тип.</param>
        /// <param name="interfaceType">Тип интерфейса.</param>
        /// <returns>Экземпляр тип прокси.</returns>
        public Type GetProxyType(Type baseProxyType, Type interfaceType)
        {
            // mapping = interface -> generated_proxy
            if (!_internalCache.TryGetValue(baseProxyType, out var mapping))
                _internalCache[baseProxyType] = mapping = new Dictionary<Type, Type>();

            if (!mapping.TryGetValue(interfaceType, out var proxyType))
                mapping[interfaceType] = proxyType = _proxyTypeGeneratorFacade
                    .GenerateProxyType(baseProxyType, interfaceType);

            return proxyType;
        }
    }
}