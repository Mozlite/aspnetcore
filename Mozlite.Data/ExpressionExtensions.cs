using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Properties;

namespace Mozlite.Data
{
    /// <summary>
    /// 表达式扩展类。
    /// </summary>
    [DebuggerStepThrough]
    public static class ExpressionExtensions
    {
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

        /// <summary>
        /// 获取属性名称列表。
        /// </summary>
        /// <param name="propertyAccessExpression">属性表达式。</param>
        /// <returns>番红花属性名称列表。</returns>
        public static string[] GetPropertyNames(this LambdaExpression propertyAccessExpression)
        {
            return propertyAccessExpression.GetPropertyAccessList().Select(p => p.Name).ToArray();
        }

        private static IReadOnlyList<PropertyInfo> MatchPropertyAccessList(
            this LambdaExpression lambdaExpression, Func<Expression, Expression, PropertyInfo> propertyMatcher)
        {
            Debug.Assert(lambdaExpression.Body != null);

            var newExpression
                = RemoveConvert(lambdaExpression.Body) as NewExpression;

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
                memberExpression = RemoveConvert(propertyAccessExpression) as MemberExpression;

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
        /// 获取表达式执行后的值。
        /// </summary>
        /// <param name="expression">当前表达式。</param>
        /// <returns>返回执行后的结果。</returns>
        public static object Invoke(this Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        ///// <summary>
        ///// 判断当前<paramref name="item"/>是否包含在<paramref name="items"/>中。
        ///// </summary>
        ///// <param name="item">当前项。</param>
        ///// <param name="items">列表实例。</param>
        ///// <returns>返回判断结果。</returns>
        //public static bool Included(this object item, IEnumerable items)
        //{
        //    var enumerator = items.GetEnumerator();
        //    do
        //    {
        //        if (ReferenceEquals(enumerator.Current, item))
        //            return true;
        //    } while (enumerator.MoveNext());
        //    return false;
        //}

        /// <summary>
        /// 添加并且条件。
        /// </summary>
        /// <typeparam name="T">当前实体类型。</typeparam>
        /// <param name="expression">原有条件表达式。</param>
        /// <param name="merger">附加并且条件表达式。</param>
        /// <returns>返回添加后得表达式。</returns>
        public static Expression<Predicate<T>> AndAlso<T>(this Expression<Predicate<T>> expression,
            Expression<Predicate<T>> merger) => expression.Merge(merger, Expression.AndAlso);

        /// <summary>
        /// 添加或者条件。
        /// </summary>
        /// <typeparam name="T">当前实体类型。</typeparam>
        /// <param name="expression">原有条件表达式。</param>
        /// <param name="merger">附加或者条件表达式。</param>
        /// <returns>返回添加后得表达式。</returns>
        public static Expression<Predicate<T>> OrElse<T>(this Expression<Predicate<T>> expression,
            Expression<Predicate<T>> merger) => expression.Merge(merger, Expression.OrElse);

        private static Expression<Predicate<T>> Merge<T>(this Expression<Predicate<T>> expression,
            Expression<Predicate<T>> merger, Func<Expression, Expression, BinaryExpression> method)
        {
            var invoker = Expression.Invoke(merger, expression.Parameters);
            return Expression.Lambda<Predicate<T>>(method(expression.Body, invoker), expression.Parameters);
        }

        /// <summary>
        /// 通过表达式过滤数据。
        /// </summary>
        /// <typeparam name="T">当前实例类型。</typeparam>
        /// <param name="values">值列表。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前实例列表。</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> values, Expression<Predicate<T>> expression)
        {
            if (values == null || expression == null)
                return values;

            var filter = expression.Compile();
            return values.Where(filter.Invoke).ToList();
        }
    }
}