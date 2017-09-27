using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query.Translators.Internal
{
    /// <summary>
    /// 转换字符串对比string.IsNullOrEmpty表达式。
    /// </summary>
    public class IsNullOrEmptyTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) });

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            return ReferenceEquals(methodCallExpression.Method, _methodInfo)
                ? new IsNullExpression(methodCallExpression.Arguments[0])
                : null;
        }
    }
}
