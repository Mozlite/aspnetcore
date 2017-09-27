using System;
using System.Data;
using System.Linq.Expressions;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// 日期转换器。
    /// </summary>
    public class DateTimeDateComponentTranslator : IMemberTranslator
    {
        public virtual Expression Translate(MemberExpression memberExpression)
            => (memberExpression.Expression != null)
               && (memberExpression.Expression.Type == typeof(DateTime))
               && (memberExpression.Member.Name == nameof(DateTime.Date))
                ? new SqlFunctionExpression("CONVERT",
                    memberExpression.Type,
                    new[]
                    {
                        Expression.Constant(DbType.Date),
                        memberExpression.Expression
                    })
                : null;
    }
}