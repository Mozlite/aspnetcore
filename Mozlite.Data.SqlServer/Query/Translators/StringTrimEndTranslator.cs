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
        // Method defined in netcoreapp2.0 only
        private static readonly MethodInfo _methodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new Type[] { });

        // Method defined in netstandard2.0
        private static readonly MethodInfo _methodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new[] { typeof(char[]) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoWithoutArgs?.Equals(methodCallExpression.Method) == true
                || _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
                // SqlServer RTRIM does not take arguments
                && ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
            {
                var sqlArguments = new[] { methodCallExpression.Object };

                return new SqlFunctionExpression("RTRIM", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}