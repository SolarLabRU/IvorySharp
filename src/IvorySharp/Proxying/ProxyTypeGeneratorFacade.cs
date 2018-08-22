using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Фасад для генерации типа прокси.
    /// </summary>
    internal sealed class ProxyTypeGeneratorFacade
    {
        private readonly ProxyAssembly _proxyAssembly;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ProxyTypeGeneratorFacade"/>.
        /// </summary>
        public ProxyTypeGeneratorFacade(ProxyAssembly proxyAssembly)
        {
            _proxyAssembly = proxyAssembly;
        }

        /// <summary>
        /// Создает тип прокси.
        /// </summary>
        /// <param name="baseProxyType">Базовый тип.</param>
        /// <param name="interfaceType">Тип, который прокси реализует.</param>
        /// <returns>Тип прокси.</returns>
        public Type GenerateProxyType(Type baseProxyType, Type interfaceType)
        {
            Debug.Assert(baseProxyType != null, "baseProxyType != null");
            Debug.Assert(interfaceType != null, "interfaceType != null");

            EnsureBaseProxyTypeValid(baseProxyType);
            EnsureInterfaceTypeValid(interfaceType);

            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            var proxyTypeGenerator = _proxyAssembly.CreateProxyTypeGenerator(baseProxyType);

            foreach (var @interface in interfaceTypeInfo.ImplementedInterfaces)
                proxyTypeGenerator.ImplementInterface(@interface);

            proxyTypeGenerator.ImplementInterface(interfaceType);

            return proxyTypeGenerator.Generate().AsType();
        }

        /// <summary>
        /// Выполняет проверку того, что базовый тип является корректным.
        /// </summary>
        /// <param name="baseProxyType">Базовый тип.</param>
        private void EnsureBaseProxyTypeValid(Type baseProxyType)
        {
            var typeInfo = baseProxyType.GetTypeInfo();

            // Базовый тип не может быть запечатанным, т.к. прокси наследуется от него
            if (typeInfo.IsSealed)
                throw new ArgumentException($"Тип '{typeInfo.FullName}' не должен быть запечатанным",
                    nameof(baseProxyType));

            // Базовый тип не может быть абстрактным.
            if (typeInfo.IsAbstract)
                throw new ArgumentException($"Тип '{typeInfo.FullName}' не должен быть абстрактным",
                    nameof(baseProxyType));

            // Базовый тип должен иметь публичный конструктор по умолчанию.
            if (!typeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
                throw new ArgumentException(
                    $"Тип '{typeInfo.FullName}' должен иметь публичный конструктор без параметров",
                    nameof(baseProxyType));
        }

        /// <summary>
        /// Выполняет проверку того, что тип интерфейса является корректным.
        /// </summary>
        /// <param name="interfaceType">Тип интерфейса.</param>
        private void EnsureInterfaceTypeValid(Type interfaceType)
        {
            var typeInfo = interfaceType.GetTypeInfo();

            if (!typeInfo.IsInterface)
                throw new ArgumentException($"Тип '{typeInfo.FullName}' должен быть интерфейсом",
                    nameof(interfaceType));
        }
    }
}