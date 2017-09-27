using System.Linq.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 语句表达式转换。
    /// </summary>
    public interface IExpressionFragmentTranslator
    {
        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="expression">当前表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        Expression Translate( Expression expression);
    }
}