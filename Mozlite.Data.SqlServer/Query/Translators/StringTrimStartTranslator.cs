using System;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// string.TrimStart。
    /// </summary>
    public class StringTrimStartTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _trimStart = typeof(string).GetTypeInfo()
            .GetDeclaredMethod(nameof(string.TrimStart));
        
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if ((_trimStart == methodCallExpression.Method)
                // SqlServer LTRIM does not take arguments
                && (((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0))
            {
                var sqlArguments = new[] { methodCallExpression.Object };
                return new SqlFunctionExpression("LTRIM", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}