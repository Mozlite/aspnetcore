using System;
using System.Linq.Expressions;

namespace Mozlite.Data.Query.Expressions
{
    /// <summary>
    /// 不可为空表达式。
    /// </summary>
    public class NotNullableExpression : Expression
    {
        private readonly Expression _operand;
        /// <summary>
        /// 初始化类<see cref="NotNullableExpression"/>。
        /// </summary>
        /// <param name="operand">表达式。</param>
        public NotNullableExpression( Expression operand)
        {
            _operand = operand;
        }

        /// <summary>
        /// 表达式。
        /// </summary>
        public virtual Expression Operand => _operand;

        /// <summary>
        /// 获取此 <see cref="T:System.Linq.Expressions.Expression"/> 的节点类型。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Linq.Expressions.ExpressionType"/> 值之一。
        /// </returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// 获取此 <see cref="T:System.Linq.Expressions.Expression"/> 表示的表达式的静态类型。
        /// </summary>
        /// <returns>
        /// 表示表达式的静态类型的 <see cref="T:System.Type"/>。
        /// </returns>
        public override Type Type => typeof(bool);

        /// <summary>
        /// 简化节点，然后对简化的表达式调用访问者委托。该方法在节点不可简化时引发异常。
        /// </summary>
        /// <returns>
        /// 要访问的表达式，或应在树中替换此表达式的表达式。
        /// </returns>
        /// <param name="visitor"><see cref="T:System.Func`2"/> 的一个实例。</param>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newExpression = visitor.Visit(_operand);

            return newExpression != _operand
                ? new NotNullableExpression(newExpression)
                : this;
        }

        /// <summary>
        /// 指示可将节点简化为更简单的节点。如果返回 true，则可以调用 Reduce() 以生成简化形式。
        /// </summary>
        /// <returns>
        /// 如果可以简化节点，则为 True；否则为 false。
        /// </returns>
        public override bool CanReduce => true;

        /// <summary>
        /// 将此节点简化为更简单的表达式。如果 CanReduce 返回 true，则它应返回有效的表达式。此方法可以返回本身必须简化的另一个节点。
        /// </summary>
        /// <returns>
        /// 已简化的表达式。
        /// </returns>
        public override Expression Reduce() => _operand;
    }
}
