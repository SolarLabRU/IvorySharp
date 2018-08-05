using System;
using System.Reflection.Emit;

namespace IvorySharp.Proxying.Emitters
{
    /// <summary>
    /// Вспомогательный класс для генерации опкодов типа StInd.
    /// </summary>
    internal static class StindEmitter
    {
        private static readonly OpCode[] StIndOpCodes =
        {
            OpCodes.Nop, //Empty = 0,
            OpCodes.Nop, //Object = 1,
            OpCodes.Nop, //DBNull = 2,
            OpCodes.Stind_I1, //Boolean = 3,
            OpCodes.Stind_I2, //Char = 4,
            OpCodes.Stind_I1, //SByte = 5,
            OpCodes.Stind_I1, //Byte = 6,
            OpCodes.Stind_I2, //Int16 = 7,
            OpCodes.Stind_I2, //UInt16 = 8,
            OpCodes.Stind_I4, //Int32 = 9,
            OpCodes.Stind_I4, //UInt32 = 10,
            OpCodes.Stind_I8, //Int64 = 11,
            OpCodes.Stind_I8, //UInt64 = 12,
            OpCodes.Stind_R4, //Single = 13,
            OpCodes.Stind_R8, //Double = 14,
            OpCodes.Nop, //Decimal = 15,
            OpCodes.Nop, //DateTime = 16,
            OpCodes.Nop, //17
            OpCodes.Stind_Ref, //String = 18,
        };

        /// <summary>
        /// Выполняет генерацию опкода StInd, соответствующего по типу <paramref name="type"/>.
        /// </summary>
        /// <param name="il">IL генератор.</param>
        /// <param name="type">Тип для которого необходимо выполнить генерацию.</param>
        public static void Emit(ILGenerator il, Type type)
        {
            var opCode = StIndOpCodes[(int) Type.GetTypeCode(type)];
            if (!opCode.Equals(OpCodes.Nop))
            {
                il.Emit(opCode);
            }
            else
            {
                il.Emit(OpCodes.Stobj, type);
            }
        }
    }
}