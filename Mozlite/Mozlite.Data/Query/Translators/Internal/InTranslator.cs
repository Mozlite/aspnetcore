using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query.Translators.Internal
{
    /// <summary>
    /// 转换In表达式。
    /// </summary>
    public class InTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _inMethodInfo
            = typeof(ExpressionExtensions).GetRuntimeMethod(nameof(ExpressionExtensions.Included), new[] { typeof(object), typeof(IEnumerable) });

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            if (ReferenceEquals(methodCallExpression.Method, _inMethodInfo))
                return new InExpression(methodCallExpression.Arguments[0], methodCallExpression.Arguments[1]);

            return null;
        }
    }
}
