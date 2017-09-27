using System.Linq.Expressions;
using Mozlite.Data.SqlServer.Query.Expressions;

namespace Mozlite.Data.SqlServer.Query
{
    /// <summary>
    /// SQLServer表达式访问器接口。
    /// </summary>
    public interface ISqlServerExpressionVisitor
    {
        /// <summary>
        /// 解析日期部分的表达式。
        /// </summary>
        /// <param name="datePartExpression">日期部分的表达式。</param>
        /// <returns>返回表达式实例。</returns>
        Expression VisitDatePartExpression(DatePartExpression datePartExpression);
    }
}