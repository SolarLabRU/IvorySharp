using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IvorySharp.Linq
{
    internal static class Expressions
    {
        public static string GetMemberName(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            switch (expression)
            {
                case MethodCallExpression mce:
                    return mce.Method.Name;
                case MemberExpression me:
                    return me.Member.Name;
                case UnaryExpression ue:
                    return GetMemberName(ue.Operand);
                default:
                    throw new NotSupportedException($"Выражение типа '{expression.NodeType}' не поддерживается/");
            }
        }

        public static string GetMemberName<T1, T2>(Expression<Func<T1, T2>> selector)
        {
            return GetMemberName(selector.Body);
        }

        public static PropertySetter CreatePropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (property.DeclaringType == null)
                throw new ArgumentException($"{nameof(property)}.{nameof(property.DeclaringType)}", nameof(property));

            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            var expression = Expression.Lambda<PropertySetter>(
                Expression.Call(
                    Expression.Convert(instance, property.DeclaringType),
                    property.SetMethod,
                    Expression.Convert(value, property.PropertyType)
                ),
                instance, value);

            return expression.Compile();
        }

        public static DefaultValueGenerator CreateDefaultValueGenerator(Type type)
        {
            return Expression.Lambda<DefaultValueGenerator>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )).Compile();
        }

        public static MethodCall CreateMethodCall(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (method.DeclaringType == null)
                throw new ArgumentException($"{nameof(method)}.{nameof(method.DeclaringType)}", nameof(method));

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

            var lambdaExpression = Expression.Lambda<MethodCall>(
                finalExpression,
                instanceParameterExpression,
                argumentsParameterExpression
            );

            return lambdaExpression.Compile();
        }
    }
}