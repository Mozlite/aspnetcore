using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// 字符串截取转换器。
    /// </summary>
    public class StringSubstringTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo = typeof(string).GetTypeInfo()
            .GetDeclaredMethods(nameof(string.Substring))
            .Single(m => m.GetParameters().Count() == 2);

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate( MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method == _methodInfo)
            {
                var sqlArguments = new[] { methodCallExpression.Object }.Concat(methodCallExpression.Arguments);
                return new SqlFunctionExpression("SUBSTRING", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}