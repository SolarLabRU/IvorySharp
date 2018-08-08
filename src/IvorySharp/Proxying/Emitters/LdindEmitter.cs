using System;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Emitters
{
    /// <summary>
    /// Вспомогательный класс для генерации опкодов типа LdInd.
    /// </summary>
    internal static class LdindEmitter
    {
        private static readonly OpCode[] LdIndOpCodes =
        {
            OpCodes.Nop, //Empty = 0,
            OpCodes.Nop, //Object = 1,
            OpCodes.Nop, //DBNull = 2,
            OpCodes.Ldind_I1, //Boolean = 3,
            OpCodes.Ldind_I2, //Char = 4,
            OpCodes.Ldind_I1, //SByte = 5,
            OpCodes.Ldind_U1, //Byte = 6,
            OpCodes.Ldind_I2, //Int16 = 7,
            OpCodes.Ldind_U2, //UInt16 = 8,
            OpCodes.Ldind_I4, //Int32 = 9,
            OpCodes.Ldind_U4, //UInt32 = 10,
            OpCodes.Ldind_I8, //Int64 = 11,
            OpCodes.Ldind_I8, //UInt64 = 12,
            OpCodes.Ldind_R4, //Single = 13,
            OpCodes.Ldind_R8, //Double = 14,
            OpCodes.Nop, //Decimal = 15,
            OpCodes.Nop, //DateTime = 16,
            OpCodes.Nop, //17
            OpCodes.Ldind_Ref //String = 18,
        };

        /// <summary>
        /// Выполняет генерацию опкода LdInd, соответствующего по типу <paramref name="type"/>.
        /// </summary>
        /// <param name="il">IL генератор.</param>
        /// <param name="type">Тип для которого необходимо выполнить генерацию.</param>
        public static void Emit(ILGenerator il, Type type)
        {
            var opCode = LdIndOpCodes[(int) Type.GetTypeCode(type)];
            if (!opCode.Equals(OpCodes.Nop))
            {
                il.Emit(opCode);
            }
            else
            {
                il.Emit(OpCodes.Ldobj, type);
            }
        }
    }
}