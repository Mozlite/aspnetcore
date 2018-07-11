using System.Linq.Expressions;

namespace Mozlite
{
    /// <summary>
    /// 表达式树。
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 执行转换表达式，并且移除表达式内部。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>返回表达式。</returns>
        public static Expression RemoveConvert(this Expression expression)
        {
            while ((expression != null)
                   && ((expression.NodeType == ExpressionType.Convert)
                       || (expression.NodeType == ExpressionType.ConvertChecked)))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }

            return expression;
        }

        /// <summary>
        /// 获取属性名称。
        /// </summary>
        /// <param name="expression">表达式数。</param>
        /// <returns>返回属性名称。</returns>
        public static string GetPropertyName(this LambdaExpression expression)
        {
            var body = expression.Body.RemoveConvert();
            if(body is MemberExpression member)
            {
                return member.Member.Name;
            }
            return null;
        }
    }
}