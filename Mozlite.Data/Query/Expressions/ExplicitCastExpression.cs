using System;
using System.Linq.Expressions;

namespace Mozlite.Data.Query.Expressions
{
    /// <summary>
    /// 转换表达式。
    /// </summary>
    public class ExplicitCastExpression: Expression
    {
        private readonly Type _type;

        /// <summary>
        /// 初始化类<see cref="ExplicitCastExpression"/>。
        /// </summary>
        /// <param name="operand">操作数。</param>
        /// <param name="type">类型。</param>
        public ExplicitCastExpression( Expression operand,  Type type)
        {
            Check.NotNull(operand, nameof(operand));
            Check.NotNull(type, nameof(type));

            Operand = operand;
            _type = type;
        }

        /// <summary>
        /// 操作数。
        /// </summary>
        public virtual Expression Operand { get; }


        /// <summary>获取此 <see cref="T:System.Linq.Expressions.Expression" /> 的节点类型。</summary>
        /// <returns>
        /// <see cref="T:System.Linq.Expressions.ExpressionType" /> 值之一。</returns>
        public override ExpressionType NodeType => ExpressionType.Extension;


        /// <summary>获取此 <see cref="T:System.Linq.Expressions.Expression" /> 表示的表达式的静态类型。</summary>
        /// <returns>表示表达式的静态类型的 <see cref="T:System.Type" />。</returns>
        public override Type Type => _type;

        /// <summary>调度到此节点类型的特定 Visit 方法。例如，<see cref="T:System.Linq.Expressions.MethodCallExpression" /> 调用 <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />。</summary>
        /// <returns>对此节点进行访问的结果。</returns>
        /// <param name="visitor">对此节点进行访问的访问者。</param>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is ISqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitExplicitCast(this)
                : base.Accept(visitor);
        }

        /// <summary>简化节点，然后对简化的表达式调用访问者委托。该方法在节点不可简化时引发异常。</summary>
        /// <returns>要访问的表达式，或应在树中替换此表达式的表达式。</returns>
        /// <param name="visitor">
        /// <see cref="T:System.Func`2" /> 的一个实例。</param>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newOperand = visitor.Visit(Operand);

            return newOperand != Operand
                ? new ExplicitCastExpression(newOperand, _type)
                : this;
        }

        /// <summary>返回 <see cref="T:System.Linq.Expressions.Expression" /> 的的文本化表示形式。</summary>
        /// <returns>
        /// <see cref="T:System.Linq.Expressions.Expression" /> 的文本化表示形式。</returns>
        public override string ToString() => "CAST(" + Operand + " AS " + _type.Name + ")";
    }
}