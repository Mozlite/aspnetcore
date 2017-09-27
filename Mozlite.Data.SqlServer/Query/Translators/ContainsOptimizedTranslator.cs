using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// Contains转换器。
    /// </summary>
    public class ContainsOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (ReferenceEquals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];
                var patternConstantExpression = patternExpression as ConstantExpression;

                var charIndexExpression = Expression.GreaterThan(
                    new SqlFunctionExpression("CHARINDEX", typeof(int), new[] { patternExpression, methodCallExpression.Object }),
                    Expression.Constant(0));

                return
                    patternConstantExpression != null
                        ? (string)patternConstantExpression.Value == string.Empty
                            ? (Expression)Expression.Constant(true)
                            : charIndexExpression
                        : Expression.OrElse(
                            charIndexExpression,
                            Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
            }

            return null;
        }
    }
}