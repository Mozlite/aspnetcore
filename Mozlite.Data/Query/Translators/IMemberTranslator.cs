using System.Linq.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 字段或属性表达式转换接口。
    /// </summary>
    public interface IMemberTranslator
    {
        /// <summary>
        /// 转换字段或属性表达式。
        /// </summary>
        /// <param name="memberExpression">转换字段或属性表达式。</param>
        /// <returns>转换后的表达式。</returns>
        Expression Translate( MemberExpression memberExpression);
    }
}