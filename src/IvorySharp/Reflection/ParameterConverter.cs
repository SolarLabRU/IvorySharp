using System;
using System.Reflection;

namespace IvorySharp.Reflection
{
    /// <summary>
    /// Вспомогательный класс для работы с параметрами метода.
    /// </summary>
    internal static class ParameterConverter
    {
        /// <summary>
        /// Возвращает массив типов параметров.
        /// </summary>
        /// <param name="parameters">Массив параметров.</param>
        /// <returns>Массив параметров.</returns>
        public static Type[] GetTypes(ParameterInfo[] parameters)
        {
            var types = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }

            return types;
        }
    }
}