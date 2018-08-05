using System;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Emitters
{
    /// <summary>
    /// Вспомогательный класс для эмита параметров.
    /// </summary>
    internal class ParametersEmitter
    {
        private readonly ILGenerator _il;
        private readonly Type[] _paramTypes;

        internal ParametersEmitter(ILGenerator il, Type[] paramTypes)
        {
            _il = il;
            _paramTypes = paramTypes;
        }

        internal void EmitGet(int i)
        {
            _il.Emit(OpCodes.Ldarg, i + 1);
        }

        internal void EmitBeginSet(int i)
        {
            _il.Emit(OpCodes.Ldarg, i + 1);
        }

        internal void EmitEndSet(int i, Type stackType)
        {
            var argType = _paramTypes[i].GetElementType();
            ConvEmitter.Emit(_il, stackType, argType);
            StindEmitter.Emit(_il, argType);
        }
    }
}