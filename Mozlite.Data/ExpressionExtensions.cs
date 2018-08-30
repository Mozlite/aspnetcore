using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Mozlite.Data
{
    /// <summary>
    /// 表达式扩展类。
    /// </summary>
    [DebuggerStepThrough]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 获取属性名称列表。
        /// </summary>
        /// <param name="propertyAccessExpression">属性表达式。</param>
        /// <returns>番红花属性名称列表。</returns>
        public static string[] GetPropertyNames(this LambdaExpression propertyAccessExpression)
        {
            return propertyAccessExpression.GetPropertyAccessList().Select(p => p.Name).ToArray();
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