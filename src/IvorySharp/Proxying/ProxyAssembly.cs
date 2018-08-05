using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using IvorySharp.Proxying.Generators;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Динамическая сборка.
    /// </summary>
    internal class ProxyAssembly
    {
        private readonly HashSet<string> _ignoreAccessCheckAssemblies;
        private readonly Lazy<TypeInfo> _ignoreAccessCheckAttributeProvider;

        /// <summary>
        /// Тип атрибута для игнорирования модификаторов доступа.
        /// </summary>
        private TypeInfo IgnoreAccessCheckAttribute => _ignoreAccessCheckAttributeProvider.Value;

        /// <summary>
        /// Компонент для построения динамической сборки.
        /// </summary>
        private readonly AssemblyBuilder _dynamicAssemblyBuilder;

        /// <summary>
        /// Компонен для построения модуля внутри динамической сборки.
        /// </summary>
        private readonly ModuleBuilder _dynamicModuleBuilder;

        /// <summary>
        /// Хранилище связей методов.
        /// </summary>
        private readonly MethodLinkStore _methodLinkStore;

        /// <summary>
        /// Идентификатор последнего сгенерированного типа прокси.
        /// Инкрементируется при генерации следующего.
        /// </summary>
        private long _lastGeneratedTypeId;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ProxyAssembly"/>.
        /// </summary>
        /// <param name="prefix">Наименование.</param>
        /// <param name="methodLinkStore">Хранилище связей методов.</param>
        public ProxyAssembly(string prefix, MethodLinkStore methodLinkStore)
        {
            _methodLinkStore = methodLinkStore;
            _ignoreAccessCheckAssemblies = new HashSet<string>();
            _ignoreAccessCheckAttributeProvider = new Lazy<TypeInfo>(GenerateIgnoreAccessCheckAttribute);

            _dynamicAssemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"{prefix}Assembly"),
                    AssemblyBuilderAccess.Run);

            _dynamicModuleBuilder =
                _dynamicAssemblyBuilder.DefineDynamicModule($"{prefix.ToLower(CultureInfo.InvariantCulture)}module");
            _lastGeneratedTypeId = 0;
        }

        /// <summary>
        /// Создает билдер прокси.
        /// </summary>
        /// <param name="baseType">Базовый тип прокси.</param>
        /// <returns>Билдер прокси.</returns>
        public ProxyTypeGenerator CreateProxyTypeGenerator(Type baseType)
        {
            Debug.Assert(baseType != null, "baseType != null");

            var nextTypeId = Interlocked.Increment(ref _lastGeneratedTypeId);
            var fullProxyName = $"ProxyOf{baseType.Name}{nextTypeId}";
            var typeBuilder = _dynamicModuleBuilder.DefineType(fullProxyName, TypeAttributes.Public, baseType);

            return new ProxyTypeGenerator(typeBuilder, this, _methodLinkStore);
        }

        /// <summary>
        /// Проверяет видимость типа <paramref name="type"/>.
        /// Если тип недоступен из динамической сборки, выполняет генерацию
        /// атрибута для игнорирования модификаторов доступа.
        /// </summary>
        /// <param name="type">Тип, видимость которого необходимо проверить.</param>
        public void EnsureTypeVisible(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsVisible)
            {
                var assemblyName = type.Assembly.GetName().Name;
                if (!_ignoreAccessCheckAssemblies.Contains(assemblyName))
                {
                    MakeAssemblyVisible(assemblyName);
                    _ignoreAccessCheckAssemblies.Add(assemblyName);
                }
            }
        }

        /// <summary>
        /// Устанавливает сборку с имененем <paramref name="assemblyName"/>
        /// видимой для текущей динамической сборки.
        /// </summary>
        /// <param name="assemblyName">Наименование сборки.</param>
        private void MakeAssemblyVisible(string assemblyName)
        {
            var ctor = IgnoreAccessCheckAttribute.DeclaredConstructors.Single();
            var customAttributeBuilder = new CustomAttributeBuilder(ctor, new object[] {assemblyName});
            _dynamicAssemblyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        /// <summary>
        /// Выполняет генерацию атрибута, позволяющего игнорировать модификаторы доступа библиотеки.
        /// </summary>
        /// <returns>Тип сгенерированного атрибута.</returns>
        private TypeInfo GenerateIgnoreAccessCheckAttribute()
        {
            var typeBuilder = _dynamicModuleBuilder.DefineType(
                "System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(Attribute));

            return new IgnoresAccessChecksToAttributeTypeGenerator(typeBuilder).Generate();
        }
    }
}