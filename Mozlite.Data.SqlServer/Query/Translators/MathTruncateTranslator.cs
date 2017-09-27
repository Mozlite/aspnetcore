using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// Math.Truncate转换器。
    /// </summary>
    public class MathTruncateTranslator : IMethodCallTranslator
    {
        private static readonly IEnumerable<MethodInfo> _methodInfos = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Truncate));

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate( MethodCallExpression methodCallExpression)
        {
            if (_methodInfos.Contains(methodCallExpression.Method))
            {
                var arguments = new[] { methodCallExpression.Arguments[0], Expression.Constant(0), Expression.Constant(1) };
                return new SqlFunctionExpression("ROUND", methodCallExpression.Type, arguments);
            }

            return null;
        }
    }
}