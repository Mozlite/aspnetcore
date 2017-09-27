using System.Linq.Expressions;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// SQL表达式访问接口。
    /// </summary>
    public interface ISqlExpressionVisitor
    {
        /// <summary>
        /// 访问IsNull表达式。
        /// </summary>
        /// <param name="isNullExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitIsNull( IsNullExpression isNullExpression);

        /// <summary>
        /// 访问Like表达式。
        /// </summary>
        /// <param name="likeExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitLike( LikeExpression likeExpression);

        /// <summary>
        /// 访问文本表达式。
        /// </summary>
        /// <param name="literalExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitLiteral( LiteralExpression literalExpression);

        /// <summary>
        /// 访问文本对比表达式。
        /// </summary>
        /// <param name="stringCompareExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitStringCompare( StringCompareExpression stringCompareExpression);

        /// <summary>
        /// SQL函数表达式。
        /// </summary>
        /// <param name="sqlFunctionExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitSqlFunction( SqlFunctionExpression sqlFunctionExpression);

        /// <summary>
        /// 访问IN表达式。
        /// </summary>
        /// <param name="inExpression">表达式。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitIn( InExpression inExpression);

        /// <summary>
        /// 访问NOT IN表达式。
        /// </summary>
        /// <param name="inExpression">表达式。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitNotIn( InExpression inExpression);

        /// <summary>
        /// 访问CAST AS表达式。
        /// </summary>
        /// <param name="explicitCastExpression">表达式。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        Expression VisitExplicitCast(ExplicitCastExpression explicitCastExpression);
    }
}