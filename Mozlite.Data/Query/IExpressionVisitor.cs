using System.Linq.Expressions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 条件表达式访问接口。
    /// </summary>
    public interface IExpressionVisitor
    {
        /// <summary>
        /// 将表达式调度到此类中更专用的访问方法之一。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="expression">要访问的表达式。</param>
        Expression Visit(Expression expression);
    }
}