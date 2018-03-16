using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IvorySharp.Extensions;

namespace IvorySharp.Reflection
{
    internal static class Expressions
    {
        public static Func<TConvert> CreateFactoryMethod<TConvert>(Type type)
        {
            if (!type.HasDefaultConstructor())
                throw new ArgumentException("Поддерживаются только типы с конструктором по умолчанию");

            if (!typeof(TConvert).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"Преобразование [{type.Name}] -> [{typeof(TConvert).Name}] не поддерживается");

            var creatorExpression = Expression.Lambda<Func<TConvert>>(
                Expression.Convert(
                    Expression.New(type), typeof(TConvert))
            );

            return creatorExpression.Compile();
        }

        public static Action<object, object> CreatePropertySetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            var expression = Expression.Lambda<Action<object, object>>(
                Expression.Call(
                    Expression.Convert(instance, property.DeclaringType),
                    property.SetMethod,
                    Expression.Convert(value, property.PropertyType)
                ), 
                instance, value);

            return expression.Compile();
        }
        
        public static Func<object, object[], object> CreateMethodInvoker(MethodInfo method)
        {
            var instanceParameterExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsParameterExpression = Expression.Parameter(typeof(object[]), "args");

            var index = 0;
            var argumentExtractionExpressions =
                method
                    .GetParameters()
                    .Select(parameter =>
                        Expression.Convert(
                            Expression.ArrayAccess(
                                argumentsParameterExpression,
                                Expression.Constant(index++)
                            ),
                            parameter.ParameterType
                        )
                    ).ToList();

            var callExpression = method.IsStatic
                ? Expression.Call(method, argumentExtractionExpressions)
                : Expression.Call(
                    Expression.Convert(
                        instanceParameterExpression,
                        method.DeclaringType
                    ),
                    method,
                    argumentExtractionExpressions
                );

            var endLabel = Expression.Label(typeof(object));
            var finalExpression = method.ReturnType == typeof(void)
                ? (Expression) Expression.Block(
                    callExpression,
                    Expression.Return(endLabel, Expression.Constant(null)),
                    Expression.Label(endLabel, Expression.Constant(null))
                )
                : Expression.Convert(callExpression, typeof(object));

            var lambdaExpression = Expression.Lambda<Func<object, object[], object>>(
                finalExpression,
                instanceParameterExpression,
                argumentsParameterExpression
            );

            return lambdaExpression.Compile();
        }
    }
}