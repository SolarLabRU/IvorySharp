﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IvorySharp.Extensions;

namespace IvorySharp.Reflection
{
    public class Expressions
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

        public static Func<object, object[], object> CreateMethodInvoker(MethodInfo pMethodInfo)
        {
            var instanceParameterExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsParameterExpression = Expression.Parameter(typeof(object[]), "args");

            var index = 0;
            var argumentExtractionExpressions =
                pMethodInfo
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

            var callExpression = pMethodInfo.IsStatic
                ? Expression.Call(pMethodInfo, argumentExtractionExpressions)
                : Expression.Call(
                    Expression.Convert(
                        instanceParameterExpression,
                        pMethodInfo.DeclaringType
                    ),
                    pMethodInfo,
                    argumentExtractionExpressions
                );

            var endLabel = Expression.Label(typeof(object));
            var finalExpression = pMethodInfo.ReturnType == typeof(void)
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