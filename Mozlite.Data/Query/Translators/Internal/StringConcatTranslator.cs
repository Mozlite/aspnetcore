using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Data.Query.Expressions;

namespace Mozlite.Data.Query.Translators.Internal
{
    /// <summary>
    /// String.Concat方法。
    /// </summary>
    public class StringConcatTranslator : IExpressionFragmentTranslator
    {
        private static readonly MethodInfo _stringConcatMethodInfo = typeof(string).GetTypeInfo()
            .GetDeclaredMethods(nameof(string.Concat))
            .Single(m => m.GetParameters().Length == 2
                         && m.GetParameters()[0].ParameterType == typeof(object)
                         && m.GetParameters()[1].ParameterType == typeof(object));
        
        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="expression">当前表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null
                && binaryExpression.NodeType == ExpressionType.Add
                && binaryExpression.Method == _stringConcatMethodInfo)
            {
                var newLeft = binaryExpression.Left.Type != typeof(string)
                    ? new ExplicitCastExpression(HandleNullTypedConstant(binaryExpression.Left.RemoveConvert()), typeof(string))
                    : binaryExpression.Left;

                var newRight = binaryExpression.Right.Type != typeof(string)
                    ? new ExplicitCastExpression(HandleNullTypedConstant(binaryExpression.Right.RemoveConvert()), typeof(string))
                    : binaryExpression.Right;

                if (newLeft != binaryExpression.Left
                    || newRight != binaryExpression.Right)
                {
                    return Expression.Add(newLeft, newRight, _stringConcatMethodInfo);
                }
            }

            return null;
        }

        private static Expression HandleNullTypedConstant(Expression expression)
        {
            var constantExpression = expression as ConstantExpression;

            return constantExpression != null && constantExpression.Type == typeof(object) && constantExpression.Value != null
                ? Expression.Constant(constantExpression.Value)
                : expression;
        }
    }
}