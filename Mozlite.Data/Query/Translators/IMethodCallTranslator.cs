using System.Linq.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 方法调用表达式转换接口。
    /// </summary>
    public interface IMethodCallTranslator
    {
        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        Expression Translate( MethodCallExpression methodCallExpression);
    }
}
