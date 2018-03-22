using System;
using System.Linq.Expressions;

namespace Mozlite.Data.Query.Expressions
{
    /// <summary>
    /// IN表达式。
    /// </summary>
    public class InExpression : Expression
    {
        /// <summary>
        /// 初始化类<see cref="InExpression"/>。
        /// </summary>
        /// <param name="operand">别名表达式。</param>
        /// <param name="values">值表达式。</param>
        public InExpression( Expression operand,  Expression values)
        {
            Check.NotNull(operand, nameof(operand));
            Check.NotNull(values, nameof(values));

            Operand = operand;
            Values = values;
        }

        /// <summary>
        /// 别名表达式。
        /// </summary>
        public virtual Expression Operand { get; }

        /// <summary>
        /// 值表达式。
        /// </summary>
        public virtual Expression Values { get; }

        /// <summary>
        /// 获取此 <see cref="T:System.Linq.Expressions.Expression"/> 的节点类型。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Linq.Expressions.ExpressionType"/> 值之一。
        /// </returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// 获取此 <see cref="T:System.Linq.Expressions.Expression"/> 所表示的表达式的静态类型。
        /// </summary>
        /// <returns>
        /// 表示表达式的静态类型的 <see cref="T:System.Type"/>。
        /// </returns>
        public override Type Type => typeof(bool);

        /// <summary>
        /// 调度到此节点类型的特定 Visit 方法。 例如，<see cref="T:System.Linq.Expressions.MethodCallExpression"/> 调用 <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)"/>。
        /// </summary>
        /// <returns>
        /// 对此节点进行访问的结果。
        /// </returns>
        /// <param name="visitor">对此节点进行访问的访问者。</param>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is ISqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitIn(this)
                : base.Accept(visitor);
        }

        /// <summary>
        /// 简化节点，然后对简化的表达式调用访问者委托。 该方法在节点不可简化时引发异常。
        /// </summary>
        /// <returns>
        /// 要访问的表达式，或应在树中替换此表达式的表达式。
        /// </returns>
        /// <param name="visitor"><see cref="T:System.Func`2"/> 的一个实例。</param>
        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        /// <summary>
        /// 返回 <see cref="T:System.Linq.Expressions.Expression"/> 的文本化表示形式。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Linq.Expressions.Expression"/> 的文本化表示形式。
        /// </returns>
        public override string ToString()
            => Operand + " IN (" + string.Join(", ", Values) + ")";
    }
}