using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Properties;

namespace Mozlite
{
    /// <summary>
    /// 表达式树。
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 执行转换表达式，并且移除表达式内部。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>返回表达式。</returns>
        public static Expression RemoveConvert(this Expression expression)
        {
            while ((expression != null)
                   && ((expression.NodeType == ExpressionType.Convert)
                       || (expression.NodeType == ExpressionType.ConvertChecked)))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }

            return expression;
        }

        /// <summary>
        /// 从表达式中获取对应的属性实例对象。
        /// </summary>
        /// <param name="propertyAccessExpression">当前属性访问的表达式。</param>
        /// <returns>防护当前表达式对应的属性实例对象。</returns>
        public static PropertyInfo GetPropertyAccess(this LambdaExpression propertyAccessExpression)
        {
            var parameterExpression = propertyAccessExpression.Parameters.Single();
            var propertyInfo = parameterExpression.MatchSimplePropertyAccess(propertyAccessExpression.Body);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(Resources.InvalidPropertyExpression, propertyAccessExpression),
                    nameof(propertyAccessExpression));
            }

            var declaringType = propertyInfo.DeclaringType;
            var parameterType = parameterExpression.Type;

            if (declaringType != null
                && declaringType != parameterType
                && declaringType.GetTypeInfo().IsInterface
                && declaringType.IsAssignableFrom(parameterType))
            {
                var propertyGetter = propertyInfo.GetGetMethod(true);
                var interfaceMapping = parameterType.GetTypeInfo().GetRuntimeInterfaceMap(declaringType);
                var index = Array.FindIndex(interfaceMapping.InterfaceMethods, p => p == propertyGetter);
                var targetMethod = interfaceMapping.TargetMethods[index];
                foreach (var runtimeProperty in parameterType.GetRuntimeProperties())
                {
                    if (targetMethod == runtimeProperty.GetGetMethod(true))
                    {
                        return runtimeProperty;
                    }
                }
            }

            return propertyInfo;
        }

        /// <summary>
        /// 从表达式中获取对应的属性实例对象集合。
        /// </summary>
        /// <param name="propertyAccessExpression">当前属性访问的表达式。</param>
        /// <returns>防护当前表达式对应的属性实例对象集合。</returns>
        public static IReadOnlyList<PropertyInfo> GetPropertyAccessList(this LambdaExpression propertyAccessExpression)
        {
            Debug.Assert(propertyAccessExpression.Parameters.Count == 1);

            var propertyPaths
                = MatchPropertyAccessList(propertyAccessExpression, (p, e) => e.MatchSimplePropertyAccess(p));

            if (propertyPaths == null)
            {
                throw new ArgumentException(string.Format(Resources.InvalidPropertiesExpression, propertyAccessExpression),
                    nameof(propertyAccessExpression));
            }

            return propertyPaths;
        }

        private static IReadOnlyList<PropertyInfo> MatchPropertyAccessList(
            this LambdaExpression lambdaExpression, Func<Expression, Expression, PropertyInfo> propertyMatcher)
        {
            Debug.Assert(lambdaExpression.Body != null);

            var newExpression
                = lambdaExpression.Body.RemoveConvert() as NewExpression;

            var parameterExpression
                = lambdaExpression.Parameters.Single();

            if (newExpression != null)
            {
                var propertyInfos
                    = newExpression
                        .Arguments
                        .Select(a => propertyMatcher(a, parameterExpression))
                        .Where(p => p != null)
                        .ToList();

                return propertyInfos.Count != newExpression.Arguments.Count ? null : propertyInfos;
            }

            var propertyPath
                = propertyMatcher(lambdaExpression.Body, parameterExpression);

            return propertyPath != null ? new[] { propertyPath } : null;
        }

        private static PropertyInfo MatchSimplePropertyAccess(
            this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyInfos = MatchPropertyAccess(parameterExpression, propertyAccessExpression);

            return (propertyInfos != null) && (propertyInfos.Count == 1) ? propertyInfos[0] : null;
        }

        private static IReadOnlyList<PropertyInfo> MatchPropertyAccess(
            this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyInfos = new List<PropertyInfo>();

            MemberExpression memberExpression;

            do
            {
                memberExpression = propertyAccessExpression.RemoveConvert() as MemberExpression;

                var propertyInfo = memberExpression?.Member as PropertyInfo;

                if (propertyInfo == null)
                {
                    return null;
                }

                propertyInfos.Insert(0, propertyInfo);

                propertyAccessExpression = memberExpression.Expression;
            }
            while (memberExpression.Expression.RemoveConvert() != parameterExpression);

            return propertyInfos;
        }

    }
}