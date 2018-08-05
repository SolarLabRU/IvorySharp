using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Emitters
{
    /// <summary>
    /// Вспомогательный класс для генерации опкодов типа Conv.
    /// </summary>
    internal static class ConvEmitter
    {
        private static readonly OpCode[] ConvOpCodes = {
            OpCodes.Nop,//Empty = 0,
            OpCodes.Nop,//Object = 1,
            OpCodes.Nop,//DBNull = 2,
            OpCodes.Conv_I1,//Boolean = 3,
            OpCodes.Conv_I2,//Char = 4,
            OpCodes.Conv_I1,//SByte = 5,
            OpCodes.Conv_U1,//Byte = 6,
            OpCodes.Conv_I2,//Int16 = 7,
            OpCodes.Conv_U2,//UInt16 = 8,
            OpCodes.Conv_I4,//Int32 = 9,
            OpCodes.Conv_U4,//UInt32 = 10,
            OpCodes.Conv_I8,//Int64 = 11,
            OpCodes.Conv_U8,//UInt64 = 12,
            OpCodes.Conv_R4,//Single = 13,
            OpCodes.Conv_R8,//Double = 14,
            OpCodes.Nop,//Decimal = 15,
            OpCodes.Nop,//DateTime = 16,
            OpCodes.Nop,//17
            OpCodes.Nop,//String = 18,
        };

        /// <summary>
        /// Выполняет генерацию опкода Conv.
        /// </summary>
        /// <param name="il">IL генератор.</param>
        /// <param name="source">Исходный тип.</param>
        /// <param name="target">Целевой тип.</param>
        public static void Emit(ILGenerator il, Type source, Type target)
        {
            if (target == source)
                return;

            var sourceTypeInfo = source.GetTypeInfo();
            var targetTypeInfo = target.GetTypeInfo();

            if (source.IsByRef)
            {
                var argType = source.GetElementType();
                LdindEmitter.Emit(il, argType);
                Emit(il, argType, target);
                return;
            }

            if (targetTypeInfo.IsValueType)
            {
                if (sourceTypeInfo.IsValueType)
                {          
                    var opCode = ConvOpCodes[(int)Type.GetTypeCode(target)];
                    il.Emit(opCode);
                }
                else
                {
                    il.Emit(OpCodes.Unbox, target);
                    LdindEmitter.Emit(il, target);
                }
            }
            else if (targetTypeInfo.IsAssignableFrom(sourceTypeInfo))
            {
                if (sourceTypeInfo.IsValueType || source.IsGenericParameter)
                {
                    il.Emit(OpCodes.Box, source);
                }
            }
            else
            {
                var opCode = target.IsGenericParameter ? OpCodes.Unbox_Any : OpCodes.Castclass;
                il.Emit(opCode, target);
            }
        }
    }
}