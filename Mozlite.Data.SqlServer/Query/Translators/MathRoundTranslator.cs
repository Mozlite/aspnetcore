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
    /// Math.Round转换器。
    /// </summary>
    public class MathRoundTranslator : IMethodCallTranslator
    {
        private static IEnumerable<MethodInfo> _methodInfos = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Round))
            .Where(m => m.GetParameters().Count() == 1
                        || (m.GetParameters().Count() == 2 && m.GetParameters()[1].ParameterType == typeof(int)));

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate( MethodCallExpression methodCallExpression)
        {
            if (_methodInfos.Contains(methodCallExpression.Method))
            {
                var arguments = methodCallExpression.Arguments.Count == 1
                    ? new[] { methodCallExpression.Arguments[0], Expression.Constant(0) }
                    : new[] { methodCallExpression.Arguments[1], methodCallExpression.Arguments[1] };

                return new SqlFunctionExpression("ROUND", methodCallExpression.Type, arguments);
            }

            return null;
        }
    }
}