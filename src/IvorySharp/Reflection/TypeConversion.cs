using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace IvorySharp.Reflection
{
    /// <summary>
    /// Helper class to handle type conversion.
    /// </summary>
    internal static class TypeConversion
    {    
        /// <summary>
        /// Converts the source object (<paramref name="value"/>) into type <paramref name="targetType"/>.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">Source object for type conversion.\</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="result">Conversion result. If conversion failed <paramref name="result"/> equals null.</param>
        /// <returns>True if <paramref name="value"/> was converted successfully, otherwise - false.</returns>
        public static bool TryConvert(object value, Type targetType, out object result)
        {
            result = null;

            if (TryConvertUnderlying(value, targetType, out var converted))
            {
                result = converted;
                return true;
            }
            
            var valueType = value.GetType();

            // Try to unpack target from Nullable{T}
            var targetUnpackedType = Nullable.GetUnderlyingType(targetType);
            if (targetUnpackedType != null)
            {
                // If we can cast types directly
                if (targetUnpackedType.IsAssignableFrom(valueType))
                {
                    // Implicit boxing (Nullable<Target>)(Target)value
                    result = value;
                    return true;
                }
            }

            // Try to unpack value from Nullable{T}
            var valueUnpackedType = Nullable.GetUnderlyingType(targetType);
            if (valueUnpackedType == null) 
                return false;
            
            // Nullable<T> -> Nullable<E> where E.IsAssignableFrom(T)
            if (targetUnpackedType == null || !targetUnpackedType.IsAssignableFrom(valueUnpackedType)) 
                return false;
            
            var coverter = 
                Expression.Lambda<Func<object>>(
                    // Boxing () => (object)[Nullable<UnpackedTarget>] value
                    Expression.Convert(
                        // const Nullable<UnpackedTarget> = (UnpackedTarget) value
                        Expression.Convert(
                            // ((TargetUnpackedType)ValueUnpackedType)value
                            Expression.Convert(
                                // (ValueUnpackedType)value
                                Expression.Convert(
                                    // const ValueType value;
                                    Expression.Constant(value, valueType),
                                    valueUnpackedType),
                                targetUnpackedType),
                            targetType),
                        typeof(object))
                );

            try
            {
                result = coverter.Compile().Invoke();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
            
        private static bool TryConvertUnderlying(object value, Type targetType, out object result)
        {
            result = null;

            if (value == null || targetType == null)
                return false;

            var valueType = value.GetType();

            if (targetType.IsAssignableFrom(valueType))
            {
                result = value;
                return true;
            }

            if (value is IConvertible convertible)
            {
                try
                {
                    result = Convert.ChangeType(convertible, targetType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var converter = TypeDescriptor.GetConverter(targetType);
            if (!converter.IsValid(value))
                return false;

            try
            {
                result = converter.ConvertFrom(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}