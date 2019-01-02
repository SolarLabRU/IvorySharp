using System;
using System.Reflection;

namespace IvorySharp.Reflection
{
    /// <summary>
    /// Helper class to handle conversion between <see cref="Type"/> and <see cref="ParameterInfo"/>.
    /// </summary>
    internal static class ParameterConverter
    {
        /// <summary>
        /// Converts an array of <see cref="ParameterInfo"/> to <see cref="Type"/>.
        /// </summary>
        /// <param name="parameters">Array of <see cref="ParameterInfo"/>.</param>
        /// <returns>Array of parameters underlying type (<see cref="Type"/>).</returns>
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