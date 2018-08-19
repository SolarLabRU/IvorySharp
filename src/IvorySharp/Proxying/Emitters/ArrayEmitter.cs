using System;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Emitters
{
    /// <summary>
    /// Вспомогательный класс для эмита массивов.
    /// </summary>
    /// <typeparam name="T">Тип массива.</typeparam>
    internal sealed class ArrayEmitter<T>
    {
        private readonly ILGenerator _ilGenerator;
        private readonly LocalBuilder _localBuilder;

        public ArrayEmitter(ILGenerator ilGenerator, int length)
        {
            _ilGenerator = ilGenerator;
            _localBuilder = ilGenerator.DeclareLocal(typeof(T[]));
            
            ilGenerator.Emit(OpCodes.Ldc_I4, length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(T));
            ilGenerator.Emit(OpCodes.Stloc, _localBuilder);
        }

        internal void EmitLoad()
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
        }
        
        internal void EmitGet(int i)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, i);
            _ilGenerator.Emit(OpCodes.Ldelem_Ref);
        }

        internal void EmitBeginSet(int i)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, i);
        }

        internal void EmitEndSet(Type stackType)
        {
            ConvEmitter.Emit(_ilGenerator, stackType, typeof(T));
            _ilGenerator.Emit(OpCodes.Stelem_Ref);
        }
    }
}