using System;
using System.Linq.Expressions;

namespace Mozlite.Data.Query.Expressions
{
    /// <summary>
    /// 字符串常量表达式。
    /// </summary>
    public class LiteralExpression : Expression
    {
        /// <summary>
        /// 初始化类<see cref="LiteralExpression"/>。
        /// </summary>
        /// <param name="literal">字符串常量。</param>
        public LiteralExpression( string literal)
        {
            Literal = literal;
        }

        /// <summary>
        /// 字符串常量。
        /// </summary>
        public virtual string Literal { get; }

        /// <summary>
        /// 调度到此节点类型的特定 Visit 方法。例如，<see cref="T:System.Linq.Expressions.MethodCallExpression"/> 调用 <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)"/>。
        /// </summary>
        /// <returns>
        /// 对此节点进行访问的结果。
        /// </returns>
        /// <param name="visitor">对此节点进行访问的访问者。</param>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is ISqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitLiteral(this)
                : base.Accept(visitor);
        }

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
        public override Type Type => typeof(string);

        /// <summary>
        /// 简化节点，然后对简化的表达式调用访问者委托。该方法在节点不可简化时引发异常。
        /// </summary>
        /// <returns>
        /// 要访问的表达式，或应在树中替换此表达式的表达式。
        /// </returns>
        /// <param name="visitor"><see cref="T:System.Func`2"/> 的一个实例。</param>
        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        /// <summary>
        /// 返回当前字符串常量。
        /// </summary>
        /// <returns>返回当前字符串常量。</returns>
        public override string ToString() => Literal;
    }
}
