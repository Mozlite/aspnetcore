﻿using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.MySql.Query.Translators
{
    /// <summary>
    /// StartsWith优化。
    /// </summary>
    public class StartsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

        private static readonly MethodInfo _concat
            = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (ReferenceEquals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];
                var patternConstantExpression = patternExpression as ConstantExpression;

                var startsWithExpression = Expression.AndAlso(
                    new LikeExpression(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        methodCallExpression.Object,
                        Expression.Add(methodCallExpression.Arguments[0], Expression.Constant("%", typeof(string)), _concat)),
                    Expression.Equal(
                        new SqlFunctionExpression("INSTR", typeof(int), new[] { patternExpression, methodCallExpression.Object }),
                        Expression.Constant(1)));

                return patternConstantExpression != null
                    ? (string)patternConstantExpression.Value == string.Empty
                        ? (Expression)Expression.Constant(true)
                        : startsWithExpression
                    : Expression.OrElse(
                        startsWithExpression,
                        Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
            }

            return null;
        }
    }
}