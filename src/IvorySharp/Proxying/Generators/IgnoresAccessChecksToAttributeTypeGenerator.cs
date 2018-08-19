using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Generators
{
    /// <summary>
    /// Выполняет генерацию атрибута для игнорирования модификаторов доступа сборки.
    /// </summary>
    internal sealed class IgnoresAccessChecksToAttributeTypeGenerator : AbstractTypeGenerator
    {
        public IgnoresAccessChecksToAttributeTypeGenerator(ModuleBuilder moduleBuilder)
            : base(CreateTypeBuilder(moduleBuilder))
        {
        }
        
        /// <inheritdoc />
        public override TypeInfo Generate()
        {
            // Create backing field as:
            // private string assemblyName;
            var assemblyNameField =
                DynamicTypeBuilder.DefineField("assemblyName", typeof(string), FieldAttributes.Private);

            // Create ctor as:
            // public IgnoresAccessChecksToAttribute(string)
            var constructorBuilder = DynamicTypeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] {assemblyNameField.FieldType});

            var il = constructorBuilder.GetILGenerator();

            // Create ctor body as:
            // this.assemblyName = {ctor parameter 0}
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg, 1);
            il.Emit(OpCodes.Stfld, assemblyNameField);

            // return
            il.Emit(OpCodes.Ret);

            // Define property as:
            // public string AssemblyName {get { return this.assemblyName; } }
            
            // ReSharper disable once UnusedVariable
            var getterPropertyBuilder = DynamicTypeBuilder.DefineProperty(
                "AssemblyName",
                PropertyAttributes.None,
                CallingConventions.HasThis,
                returnType: typeof(string),
                parameterTypes: null);

            var getterMethodBuilder = DynamicTypeBuilder.DefineMethod(
                "get_AssemblyName",
                MethodAttributes.Public,
                CallingConventions.HasThis,
                returnType: typeof(string),
                parameterTypes: null);

            // Generate body:
            // return this.assemblyName;
            il = getterMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, assemblyNameField);
            il.Emit(OpCodes.Ret);

            // Generate the AttributeUsage attribute for this attribute type:
            // [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
            var attributeUsageTypeInfo = typeof(AttributeUsageAttribute).GetTypeInfo();

            // Find the ctor that takes only AttributeTargets
            var attributeUsageConstructorInfo =
                attributeUsageTypeInfo.DeclaredConstructors
                    .Single(c => c.GetParameters().Length == 1 &&
                                 c.GetParameters()[0].ParameterType == typeof(AttributeTargets));

            // Find the property to set AllowMultiple
            var allowMultipleProperty =
                attributeUsageTypeInfo.DeclaredProperties
                    .Single(f => string.Equals(f.Name, "AllowMultiple"));

            // Create a builder to construct the instance via the ctor and property
            var customAttributeBuilder =
                new CustomAttributeBuilder(attributeUsageConstructorInfo,
                    new object[] {AttributeTargets.Assembly},
                    new[] { allowMultipleProperty },
                    new object[] {true});

            // Attach this attribute instance to the newly defined attribute type
            DynamicTypeBuilder.SetCustomAttribute(customAttributeBuilder);

            // Make the TypeInfo real so the constructor can be used.
            return DynamicTypeBuilder.CreateTypeInfo();
        }
        
        private static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                "System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(Attribute));
        }
    }
}