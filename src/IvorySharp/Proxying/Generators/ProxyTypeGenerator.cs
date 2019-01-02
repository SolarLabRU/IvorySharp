using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IvorySharp.Comparers;
using IvorySharp.Linq;
using IvorySharp.Proxying.Accessors;
using IvorySharp.Proxying.Emitters;
using IvorySharp.Reflection;

namespace IvorySharp.Proxying.Generators
{
    /// <summary>
    /// Генератор для создания типа прокси.
    /// </summary>
    internal sealed class ProxyTypeGenerator : AbstractTypeGenerator
    {
        private readonly FieldBuilder _invokeDelegateField;
        private readonly ProxyAssembly _proxyAssembly;
        private readonly MethodLinkStore _methodLinkStore;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ProxyTypeGenerator"/>
        /// </summary>
        public ProxyTypeGenerator(
            TypeBuilder dynamicTypeBuilder,
            ProxyAssembly proxyAssembly,
            MethodLinkStore methodLinkStore)
            : base(dynamicTypeBuilder)
        {
            _proxyAssembly = proxyAssembly;
            _methodLinkStore = methodLinkStore;

            _invokeDelegateField = DynamicTypeBuilder.DefineField("invoke", typeof(Action<object[]>),
                FieldAttributes.Private);
        }

        /// <summary>
        /// Добавляет реализацию интерфейса к прокси.
        /// </summary>
        /// <param name="interfaceType">Тип интерфейса.</param>
        public void ImplementInterface(Type interfaceType)
        {
            // Если необходимо - добавляем в сборку атрибут игнорирующий модификаторы доступа
            _proxyAssembly.EnsureTypeVisible(interfaceType);

            DynamicTypeBuilder.AddInterfaceImplementation(interfaceType);

            // AccessorMethods -> Metadata mappings.
            var propertyMap = new Dictionary<MethodInfo, PropertyAccessor>(MethodEqualityComparer.Instance);
            foreach (var pi in interfaceType.GetRuntimeProperties())
            {
                var ai = new PropertyAccessor(pi.GetMethod, pi.SetMethod);
                if (pi.GetMethod != null)
                    propertyMap[pi.GetMethod] = ai;
                if (pi.SetMethod != null)
                    propertyMap[pi.SetMethod] = ai;
            }

            var eventMap = new Dictionary<MethodInfo, EventAccessor>(MethodEqualityComparer.Instance);
            foreach (var ei in interfaceType.GetRuntimeEvents())
            {
                var ai = new EventAccessor(ei.AddMethod, ei.RemoveMethod, ei.RaiseMethod);
                if (ei.AddMethod != null)
                    eventMap[ei.AddMethod] = ai;
                if (ei.RemoveMethod != null)
                    eventMap[ei.RemoveMethod] = ai;
                if (ei.RaiseMethod != null)
                    eventMap[ei.RaiseMethod] = ai;
            }

            foreach (var mi in interfaceType.GetRuntimeMethods())
            {
                var mdb = ImplementMethod(mi);
                if (propertyMap.TryGetValue(mi, out var associatedProperty))
                {
                    if (MethodEqualityComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, mi))
                        associatedProperty.GetMethodBuilder = mdb;
                    else
                        associatedProperty.SetMethodBuilder = mdb;
                }

                // ReSharper disable once InvertIf
                if (eventMap.TryGetValue(mi, out var associatedEvent))
                {
                    if (MethodEqualityComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, mi))
                        associatedEvent.AddMethodBuilder = mdb;
                    else if (MethodEqualityComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, mi))
                        associatedEvent.RemoveMethodBuilder = mdb;
                    else
                        associatedEvent.RaiseMethodBuilder = mdb;
                }
            }

            foreach (var pi in interfaceType.GetRuntimeProperties())
            {
                var ai = propertyMap[pi.GetMethod ?? pi.SetMethod];
                var pb = DynamicTypeBuilder.DefineProperty(pi.Name, pi.Attributes, pi.PropertyType,
                    pi.GetIndexParameters().Select(p => p.ParameterType).ToArray());
                if (ai.GetMethodBuilder != null)
                    pb.SetGetMethod(ai.GetMethodBuilder);
                if (ai.SetMethodBuilder != null)
                    pb.SetSetMethod(ai.SetMethodBuilder);
            }

            foreach (var ei in interfaceType.GetRuntimeEvents())
            {
                var ai = eventMap[ei.AddMethod ?? ei.RemoveMethod];
                var eb = DynamicTypeBuilder.DefineEvent(ei.Name, ei.Attributes, ei.EventHandlerType);
                if (ai.AddMethodBuilder != null)
                    eb.SetAddOnMethod(ai.AddMethodBuilder);
                if (ai.RemoveMethodBuilder != null)
                    eb.SetRemoveOnMethod(ai.RemoveMethodBuilder);
                if (ai.RaiseMethodBuilder != null)
                    eb.SetRaiseMethod(ai.RaiseMethodBuilder);
            }
        }

        /// <inheritdoc />
        public override TypeInfo Generate()
        {
            var argType = _invokeDelegateField.FieldType;          
            var proxyCtor = DynamicTypeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.HasThis, new [] { argType });

            var badeType = DynamicTypeBuilder.BaseType.GetTypeInfo();
            var baseCtor = badeType.DeclaredConstructors.Single(c => c.IsPublic && c.GetParameters().Length == 0);
            var ilGenerator = proxyCtor.GetILGenerator();
            
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseCtor);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, _invokeDelegateField);
            ilGenerator.Emit(OpCodes.Ret);

            return DynamicTypeBuilder.CreateTypeInfo();
        }

        /// <summary>
        /// Делает целевой метод обобщенным (generic), если исходный является таковым.
        /// </summary>
        /// <param name="source">Исходный метод.</param>
        /// <param name="target">Целевой метод.</param>
        private static void MakeGeneric(MethodBase source, MethodBuilder target)
        {
            if (!source.ContainsGenericParameters)
                return;

            var sourceGenericArguments = source.GetGenericArguments();
            var sourceGenericArgumentNames = new string[sourceGenericArguments.Length];
            for (var i = 0; i < sourceGenericArguments.Length; i++)
            {
                sourceGenericArgumentNames[i] = sourceGenericArguments[i].Name;
            }

            var genericParameters = target.DefineGenericParameters(sourceGenericArgumentNames);
            for (var i = 0; i < genericParameters.Length; i++)
            {
                genericParameters[i].SetGenericParameterAttributes(
                    sourceGenericArguments[i].GetTypeInfo().GenericParameterAttributes);
            }
        }

        /// <summary>
        /// Добавляет к типу прокси реализацию метода.
        /// </summary>
        /// <param name="method">Исходный метод.</param>
        /// <returns>Динамический метод прокси.</returns>
        private MethodBuilder ImplementMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var paramTypes = ParameterConverter.GetTypes(parameters);
            var proxiedMethod = DynamicTypeBuilder.DefineMethod(method.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType, paramTypes);

            MakeGeneric(method, proxiedMethod);
            
            var ilGenerator = proxiedMethod.GetILGenerator();
            var parametersEmitter = new ParametersEmitter(ilGenerator, paramTypes);
            var packedArgsEmitter = new PackedArgumentsEmitter(ilGenerator);

            ilGenerator.Emit(OpCodes.Nop);

            packedArgsEmitter.EmitProxy();

            // Обобщенные параметры будут известны только во время вызова
            // поэтому заранее собрать делегат не выйдетю
            var methodLambda = method.ContainsGenericParameters
                ? new ExtendedMethodInfo(method, null)
                : new ExtendedMethodInfo(method, Expressions.CreateMethodCall(method));
            
            var token = _methodLinkStore.CreateToken(methodLambda);
            packedArgsEmitter.EmitMethodToken(token);
            
            var argumentsEmitter = packedArgsEmitter.EmitArguments(parameters, parametersEmitter);
            
            if (method.ContainsGenericParameters)
            {
                packedArgsEmitter.EmitGenericArguments(method.GetGenericArguments());
            }
            
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, _invokeDelegateField);
            packedArgsEmitter.EmitLoad();
            ilGenerator.Emit(OpCodes.Call, MethodReferences.ActionOfObjectArrayInvoke);

            // Странный блок, не уверен что он нужен. 
            for (var i = 0; i < parameters.Length; i++)
            {
                // ReSharper disable once InvertIf
                if (parameters[i].ParameterType.IsByRef)
                {
                    parametersEmitter.EmitBeginSet(i);
                    argumentsEmitter.EmitGet(i);
                    parametersEmitter.EmitEndSet(i, typeof(object));
                }
            }

            packedArgsEmitter.EmitReturnValue(method.ReturnType);

            DynamicTypeBuilder.DefineMethodOverride(proxiedMethod, method);
            return proxiedMethod;
        }

        /// <summary>
        /// Вспомогательный класс для эмита отдельных фрагментов <see cref="PackedArguments"/>.
        /// </summary>
        private class PackedArgumentsEmitter
        {
            private readonly ILGenerator _ilGenerator;
            private readonly ArrayEmitter<object> _internalEmitter;

            public PackedArgumentsEmitter(ILGenerator ilGenerator)
            {
                _ilGenerator = ilGenerator;
                _internalEmitter = new ArrayEmitter<object>(ilGenerator, PackedArguments.Count);
            }

            public void EmitLoad()
            {
                _internalEmitter.EmitLoad();
            }

            public void EmitProxy()
            {
                // packed[PackedArgs.DispatchProxyPosition] = this;
                _internalEmitter.EmitBeginSet((int) PackedArgumentPosition.Proxy);
                _ilGenerator.Emit(OpCodes.Ldarg_0);
                _internalEmitter.EmitEndSet(typeof(IvoryProxy));
            }

            public void EmitMethodToken(MethodToken token)
            {
                _internalEmitter.EmitBeginSet((int) PackedArgumentPosition.DeclaringType);
                _ilGenerator.Emit(OpCodes.Ldtoken, token.DeclaringType);
                _ilGenerator.Emit(OpCodes.Call, MethodReferences.GetTypeFromHandle);
                _internalEmitter.EmitEndSet(typeof(object));

                // packed[PackedArgs.MethodTokenPosition] = iface method token;
                _internalEmitter.EmitBeginSet((int) PackedArgumentPosition.MethodTokenKey);
                _ilGenerator.Emit(OpCodes.Ldc_I4, token.Key);
                _internalEmitter.EmitEndSet(typeof(int));
            }

            public ArrayEmitter<object> EmitArguments(IReadOnlyList<ParameterInfo> parameters, ParametersEmitter parametersEmitter)
            {
                // object[] args = new object[paramCount];
                _ilGenerator.Emit(OpCodes.Nop);

                _internalEmitter.EmitBeginSet((int) PackedArgumentPosition.MethodArguments);

                var argsEmitter = new ArrayEmitter<object>(_ilGenerator, parameters.Count);

                for (var i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].IsOut) 
                        continue;
                    
                    // args[i] = argi;
                    argsEmitter.EmitBeginSet(i);
                    parametersEmitter.EmitGet(i);
                    argsEmitter.EmitEndSet(parameters[i].ParameterType);
                }

                argsEmitter.EmitLoad();

                _internalEmitter.EmitEndSet(typeof(object[]));
                
                return argsEmitter;
            }

            public void EmitGenericArguments(IReadOnlyList<Type> genericArgs)
            {
                _internalEmitter.EmitBeginSet((int) PackedArgumentPosition.GenericParameters);

                var typesEmitter = new ArrayEmitter<Type>(_ilGenerator, genericArgs.Count);

                for (var i = 0; i < genericArgs.Count; ++i)
                {
                    typesEmitter.EmitBeginSet(i);
                    _ilGenerator.Emit(OpCodes.Ldtoken, genericArgs[i]);
                    _ilGenerator.Emit(OpCodes.Call, MethodReferences.GetTypeFromHandle);
                    typesEmitter.EmitEndSet(typeof(Type));
                }

                typesEmitter.EmitLoad();

                _internalEmitter.EmitEndSet(typeof(Type[]));
            }

            public void EmitReturnValue(Type returnType)
            {
                if (returnType != typeof(void))
                {
                    _internalEmitter.EmitGet((int) PackedArgumentPosition.ReturnValue);
                    ConvEmitter.Emit(_ilGenerator, typeof(object), returnType);
                }

                _ilGenerator.Emit(OpCodes.Ret);
            }
        }
    }
}