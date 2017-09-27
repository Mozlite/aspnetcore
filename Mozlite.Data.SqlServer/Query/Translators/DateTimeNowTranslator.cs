using System;
using System.Linq;
using System.Linq.Expressions;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// DateTime.Now转换。
    /// </summary>
    public class DateTimeNowTranslator : IMemberTranslator
    {
        /// <summary>
        /// 转换字段或属性表达式。
        /// </summary>
        /// <param name="memberExpression">转换字段或属性表达式。</param>
        /// <returns>转换后的表达式。</returns>
        public virtual Expression Translate( MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime)
                && memberExpression.Member.Name == nameof(DateTime.Now))
            {
                return new SqlFunctionExpression("GETDATE", memberExpression.Type, Enumerable.Empty<Expression>());
            }

            return null;
        }
    }
}