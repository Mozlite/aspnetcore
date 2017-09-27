using System;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// string.TrimEnd.
    /// </summary>
    public class StringTrimEndTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _trimEnd = typeof(string).GetTypeInfo()
            .GetDeclaredMethod(nameof(string.TrimEnd));
        
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if ((_trimEnd == methodCallExpression.Method)
                // SqlServer RTRIM does not take arguments
                && (((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0))
            {
                var sqlArguments = new[] { methodCallExpression.Object };
                return new SqlFunctionExpression("RTRIM", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}