using System.Linq.Expressions;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// 字符串长度转换器。
    /// </summary>
    public class StringLengthTranslator : IMemberTranslator
    {
        /// <summary>
        /// 转换字段或属性表达式。
        /// </summary>
        /// <param name="memberExpression">转换字段或属性表达式。</param>
        /// <returns>转换后的表达式。</returns>
        public virtual Expression Translate( MemberExpression memberExpression)
        {
            if (memberExpression.Expression != null
                && memberExpression.Expression.Type == typeof(string)
                && memberExpression.Member.Name == nameof(string.Length))
            {
                return new SqlFunctionExpression("LEN", memberExpression.Type, new[] { memberExpression.Expression });
            }

            return null;
        }
    }
}