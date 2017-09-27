using System.Linq.Expressions;

namespace Mozlite.Data.Query.Translators.Internal
{
    /// <summary>
    /// Equals表达式转换接口。
    /// </summary>
    public class EqualsTranslator : IMethodCallTranslator
    {
        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            if (methodCallExpression.Method.Name == nameof(object.Equals)
                && methodCallExpression.Arguments.Count == 1)
            {
                var argument = methodCallExpression.Arguments[0];
                var @object = methodCallExpression.Object;
                if (methodCallExpression.Method.GetParameters()[0].ParameterType == typeof(object)
                    && @object.Type != argument.Type)
                {
                    argument = argument.RemoveConvert();
                    var unwrappedObjectType = @object.Type.UnwrapNullableType();
                    var unwrappedArgumentType = argument.Type.UnwrapNullableType();
                    if (unwrappedObjectType == unwrappedArgumentType)
                    {
                        return Expression.Equal(
                            Expression.Convert(@object, unwrappedObjectType),
                            Expression.Convert(argument, unwrappedArgumentType));
                    }

                    return Expression.Constant(false);
                }

                return Expression.Equal(methodCallExpression.Object, argument);
            }

            return null;
        }
    }
}
