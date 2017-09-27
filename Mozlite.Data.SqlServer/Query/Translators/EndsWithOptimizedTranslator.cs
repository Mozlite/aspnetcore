using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// EndsWith优化。
    /// </summary>
    public class EndsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (ReferenceEquals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];
                var patternConstantExpression = patternExpression as ConstantExpression;

                var endsWithExpression = Expression.Equal(
                    new SqlFunctionExpression(
                        "RIGHT",
                        // ReSharper disable once PossibleNullReferenceException
                        methodCallExpression.Object.Type,
                        new[]
                        {
                            methodCallExpression.Object,
                            new SqlFunctionExpression("LEN", typeof(int), new[] { patternExpression })
                        }),
                    patternExpression);

                return new NotNullableExpression(
                    patternConstantExpression != null
                        ? (string)patternConstantExpression.Value == string.Empty
                            ? (Expression)Expression.Constant(true)
                            : endsWithExpression
                        : Expression.OrElse(
                            endsWithExpression,
                            Expression.Equal(patternExpression, Expression.Constant(string.Empty))));
            }

            return null;
        }
    }
}