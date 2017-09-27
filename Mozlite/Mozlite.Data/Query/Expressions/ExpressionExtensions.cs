using System.Linq.Expressions;

namespace Mozlite.Data.Query.Expressions
{
    /// <summary>
    /// 表达式扩展类。
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 是否为逻辑操作。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsLogicalOperation( this Expression expression)
        {
            Check.NotNull(expression, nameof(expression));

            return expression.NodeType == ExpressionType.AndAlso
                   || expression.NodeType == ExpressionType.OrElse;
        }

        /// <summary>
        /// 是否为简单表达式，可以直接转换。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsSimpleExpression( this Expression expression)
        {
            var unwrappedExpression = expression.RemoveConvert();

            if (unwrappedExpression is ConstantExpression
                || unwrappedExpression is ParameterExpression
                || unwrappedExpression is LiteralExpression)
                return true;

            if (unwrappedExpression.NodeType == ExpressionType.MemberAccess && unwrappedExpression.Type == typeof(bool))
                return true;
            return false;
        }
    }
}