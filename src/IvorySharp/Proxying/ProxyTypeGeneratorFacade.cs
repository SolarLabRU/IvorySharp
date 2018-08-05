using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Создает тип прокси класса.
    /// </summary>
    internal class ProxyTypeGeneratorFacade
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
        /// <param name="baseType">Базовый тип.</param>
        /// <param name="interfaceType">Тип, который прокси реализует.</param>
        /// <remarks>
        ///   Иерархия:
        ///     interface IService { }
        ///     dynamic_class Proxy : BaseType : IService { }
        ///     class BaseType : DispatchProxy { }
        /// </remarks>
        /// <returns>Тип прокси.</returns>
        public Type GenerateProxyType(Type baseType, Type interfaceType)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(interfaceType != null, "interfaceType != null");

            EnsureBaseTypeValid(baseType);
            EnsureInterfaceTypeValid(interfaceType);

            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            var proxyTypeGenerator = _proxyAssembly.CreateProxyTypeGenerator(baseType);

            foreach (var @interface in interfaceTypeInfo.ImplementedInterfaces)
                proxyTypeGenerator.ImplementInterface(@interface);

            proxyTypeGenerator.ImplementInterface(interfaceType);

            return proxyTypeGenerator.Generate().AsType();
        }

        /// <summary>
        /// Выполняет проверку того, что базовый тип является корректным.
        /// </summary>
        /// <param name="baseType">Базовый тип.</param>
        private void EnsureBaseTypeValid(Type baseType)
        {
            var typeInfo = baseType.GetTypeInfo();

            // Базовый тип не может быть запечатанным, т.к. прокси наследуется от него
            if (typeInfo.IsSealed)
                throw new ArgumentException($"Базовый тип '{typeInfo.FullName}' не должен быть запечатанным",
                    nameof(baseType));

            // Базовый тип не может быть абстрактным.
            if (typeInfo.IsAbstract)
                throw new ArgumentException($"Базовый тип '{typeInfo.FullName}' не должен быть абстрактным",
                    nameof(baseType));

            // Базовый тип должен иметь публичный конструктор по умолчанию.
            if (!typeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
                throw new ArgumentException(
                    $"Базовый тип '{typeInfo.FullName}' должен иметь публичный конструктор без параметров",
                    nameof(baseType));
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