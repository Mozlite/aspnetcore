using System.Linq.Expressions;
using Mozlite.Data.MySql.Query.Expressions;

namespace Mozlite.Data.MySql.Query
{
    /// <summary>
    /// SQLServer表达式访问器接口。
    /// </summary>
    public interface IMySqlExpressionVisitor
    {
        /// <summary>
        /// 解析日期部分的表达式。
        /// </summary>
        /// <param name="datePartExpression">日期部分的表达式。</param>
        /// <returns>返回表达式实例。</returns>
        Expression VisitDatePartExpression(DatePartExpression datePartExpression);
    }
}