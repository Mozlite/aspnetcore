using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// string.Trim。
    /// </summary>
    public class StringTrimTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _trim = typeof(string).GetTypeInfo()
            .GetDeclaredMethods(nameof(string.Trim))
            .SingleOrDefault(m => !m.GetParameters().Any());
        
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_trim == methodCallExpression.Method)
            {
                var sqlArguments = new[] { methodCallExpression.Object };
                return new SqlFunctionExpression(
                    "LTRIM",
                    methodCallExpression.Type,
                    new[]
                    {
                        new SqlFunctionExpression(
                            "RTRIM",
                            methodCallExpression.Type,
                            sqlArguments)
                    });
            }

            return null;
        }
    }
}