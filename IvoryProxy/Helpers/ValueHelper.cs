using System;
using System.Linq.Expressions;

namespace IvoryProxy.Helpers
{
    internal static class ValueHelper
    {
        /// <summary>
        /// Возвращает значение по умолчанию для объекта по типу.
        /// </summary>
        /// <param name="type">Тип объекта.</param>
        /// <returns>Значение по умолчанию.</returns>
        public static object GetDefault(Type type)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(void))
                return default(object);
            
            var expression = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )
            );

            var valueProvider = expression.Compile();
            return valueProvider();
        }
    }
}