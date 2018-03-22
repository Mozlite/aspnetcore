using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;
using System.Linq;
using System.Reflection;
using Mozlite.Data.Migrations;
using Mozlite.Data.Properties;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 条件表达式解析器。
    /// </summary>
    public abstract class SqlExpressionVisitor : ExpressionVisitor, IExpressionVisitor, ISqlExpressionVisitor
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly ITypeMapper _typeMapper;
        private readonly IMemberTranslator _memberTranslator;
        private readonly IMethodCallTranslator _methodCallTranslator;
        private readonly IExpressionFragmentTranslator _fragmentTranslator;
        private readonly IndentedStringBuilder _builder = new IndentedStringBuilder();
        protected readonly Func<MemberInfo, Type, string> Delimter;

        /// <summary>
        /// 初始化类<see cref="SqlExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        protected SqlExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator)
        {
            _sqlHelper = sqlHelper;
            _typeMapper = typeMapper;
            _memberTranslator = memberTranslator;
            _methodCallTranslator = methodCallTranslator;
            _fragmentTranslator = fragmentTranslator;
            Delimter = (info, _) => sqlHelper.DelimitIdentifier(info.Name);
        }

        /// <summary>
        /// 初始化类<see cref="SqlExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        /// <param name="schemaFunc">获取前缀代理方法。</param>
        protected SqlExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator, Func<Type, string> schemaFunc)
        {
            _sqlHelper = sqlHelper;
            _typeMapper = typeMapper;
            _memberTranslator = memberTranslator;
            _methodCallTranslator = methodCallTranslator;
            _fragmentTranslator = fragmentTranslator;
            Delimter = (info, type) => sqlHelper.DelimitIdentifier(info.Name, schemaFunc(type));
        }

        /// <summary>
        /// 将表达式调度到此类中更专用的访问方法之一。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="expression">要访问的表达式。</param>
        public override Expression Visit(Expression expression)
        {
            var translatedExpression = _fragmentTranslator.Translate(expression);
            if (translatedExpression != null && translatedExpression != expression)
                return Visit(translatedExpression);

            return base.Visit(expression);
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.BinaryExpression"/> 的子级。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="binaryExpression">要访问的表达式。</param>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            Check.NotNull(binaryExpression, nameof(binaryExpression));

            if (binaryExpression.NodeType == ExpressionType.Coalesce)
            {
                _builder.Append("COALESCE(");
                Visit(binaryExpression.Left);
                _builder.Append(", ");
                Visit(binaryExpression.Right);
                _builder.Append(")");
            }
            else
            {
                var needParentheses
                    = !binaryExpression.Left.IsSimpleExpression()
                      || !binaryExpression.Right.IsSimpleExpression()
                      || binaryExpression.IsLogicalOperation();

                if (needParentheses)
                {
                    _builder.Append("(");
                }

                Visit(binaryExpression.Left);

                if (binaryExpression.IsLogicalOperation()
                    && binaryExpression.Left.IsSimpleExpression())
                {
                    _builder.Append(" = ");
                    _builder.Append(_sqlHelper.EscapeLiteral(true));
                }

                if (!TryGenerateBinaryOperator(binaryExpression.NodeType, out var op))
                    throw new ArgumentOutOfRangeException();

                if ((binaryExpression.NodeType == ExpressionType.NotEqual ||
                    binaryExpression.NodeType == ExpressionType.Equal) && binaryExpression.Right.NodeType == ExpressionType.Constant)
                {
                    var value = binaryExpression.Right.Invoke();
                    if (value == null)
                        _builder.Append(binaryExpression.NodeType == ExpressionType.Equal
                            ? " IS NULL "
                            : " IS NOT NULL ");
                    else
                    {
                        _builder.Append(op);
                        _builder.Append(_sqlHelper.EscapeLiteral(value));
                    }
                }
                else
                {
                    _builder.Append(op);

                    Visit(binaryExpression.Right);

                    if (binaryExpression.IsLogicalOperation()
                        && binaryExpression.Right.IsSimpleExpression())
                    {
                        _builder.Append(" = ");
                        _builder.Append(_sqlHelper.EscapeLiteral(true));
                    }
                }

                if (needParentheses)
                {
                    _builder.Append(")");
                }
            }

            return binaryExpression;
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.UnaryExpression"/> 的子级。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="unaryExpression">要访问的表达式。</param>
        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            Check.NotNull(unaryExpression, nameof(unaryExpression));

            if (unaryExpression.NodeType == ExpressionType.Not)
            {
                return VisitNot(unaryExpression);
            }

            if (unaryExpression.NodeType == ExpressionType.Convert)
            {
                Visit(unaryExpression.Operand);

                return unaryExpression;
            }

            return base.VisitUnary(unaryExpression);
        }

        /// <summary>
        /// 访问NOT相关表达式。
        /// </summary>
        /// <param name="unaryExpression">一元运算符表达式。</param>
        /// <returns>返回表达式。</returns>
        protected virtual Expression VisitNot(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression)
            {
                var expr = memberExpression.Expression;
                if (expr.NodeType == ExpressionType.Convert)
                    expr = expr.RemoveConvert();
                switch (expr.NodeType)
                {
                    case ExpressionType.Parameter:
                        Visit(Expression.Equal(memberExpression, Expression.Constant(false)));
                        break;
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Constant:
                        AppendInvoke(unaryExpression);
                        break;
                }
                return unaryExpression;
            }

            var expression = unaryExpression.Operand;
            if (expression is MethodCallExpression methodCallExpression)
                expression = _methodCallTranslator.Translate(methodCallExpression);

            if (expression is InExpression inExpression)
                return VisitNotIn(inExpression);

            if (expression is IsNullExpression isNullExpression)
                return VisitIsNotNull(isNullExpression);

            return unaryExpression;
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.ConditionalExpression"/> 的子级。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="expression">要访问的表达式。</param>
        protected override Expression VisitConditional(ConditionalExpression expression)
        {
            Check.NotNull(expression, nameof(expression));

            _builder.AppendLine("CASE");

            using (_builder.Indent())
            {
                _builder.AppendLine("WHEN");

                using (_builder.Indent())
                {
                    _builder.Append("(");

                    Visit(expression.Test);

                    _builder.AppendLine(")");
                }

                _builder.Append("THEN ");

                if (expression.IfTrue is ConstantExpression constantIfTrue
                    && constantIfTrue.Type == typeof(bool))
                {
                    _builder.Append((bool)constantIfTrue.Value ? "1" : "0");
                }
                else
                {
                    Visit(expression.IfTrue);
                }

                _builder.Append(" ELSE ");

                if (expression.IfFalse is ConstantExpression constantIfFalse
                    && constantIfFalse.Type == typeof(bool))
                {
                    _builder.Append((bool)constantIfFalse.Value ? "1" : "0");
                }
                else
                {
                    Visit(expression.IfFalse);
                }

                _builder.AppendLine();
            }

            _builder.Append("END");

            return expression;
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.MemberExpression"/> 的子级。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="node">要访问的表达式。</param>
        protected override Expression VisitMember(MemberExpression node)
        {
            Check.NotNull(node, nameof(node));

            var translatedExpression = _memberTranslator.Translate(node);
            if (translatedExpression != null)
                return Visit(translatedExpression);

            var expr = node.Expression;
            if (expr.NodeType == ExpressionType.Convert)
                expr = expr.RemoveConvert();
            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                    _builder.Append(Delimter(node.Member, expr.Type));
                    break;
                case ExpressionType.MemberAccess:
                case ExpressionType.Constant:
                    AppendInvoke(node);
                    break;
            }
            return node;
        }

        private void AppendInvoke(Expression expression)
        {
            var value = expression.Invoke();
            if (value == null)
                _builder.Append("null");
            else
                _builder.Append(_sqlHelper.EscapeLiteral(value));
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.MethodCallExpression"/> 的子级。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="methodCallExpression">要访问的表达式。</param>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            var translatedExpression = _methodCallTranslator.Translate(methodCallExpression);

            if (translatedExpression != null)
                return Visit(translatedExpression);

            return base.VisitMethodCall(methodCallExpression);
        }

        /// <summary>
        /// 访问 <see cref="T:System.Linq.Expressions.ConstantExpression"/>。
        /// </summary>
        /// <returns>
        /// 如果修改了该表达式或任何子表达式，则为修改后的表达式；否则返回原始表达式。
        /// </returns>
        /// <param name="constantExpression">要访问的表达式。</param>
        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            Check.NotNull(constantExpression, nameof(constantExpression));

            _builder.Append(_sqlHelper.EscapeLiteral(constantExpression.Value));

            return constantExpression;
        }

        private static readonly IDictionary<ExpressionType, string> _binaryOperatorMap = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Add, " + " },
            { ExpressionType.Equal, " = " },
            { ExpressionType.NotEqual, " <> " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.LessThanOrEqual, " <= " },
            { ExpressionType.AndAlso, " AND " },
            { ExpressionType.OrElse, " OR " },
            { ExpressionType.Subtract, " - " },
            { ExpressionType.Multiply, " * " },
            { ExpressionType.Divide, " / " },
            { ExpressionType.Modulo, " % " },
        };

        /// <summary>
        /// 获取操作符号。
        /// </summary>
        /// <param name="op">当前表达式类型。</param>
        /// <param name="result">返回操作符号。</param>
        /// <returns>返回是否有结果。</returns>
        protected virtual bool TryGenerateBinaryOperator(ExpressionType op, out string result)
        {
            return _binaryOperatorMap.TryGetValue(op, out result);
        }

        /// <summary>
        /// 获取操作符号。
        /// </summary>
        /// <param name="op">当前表达式类型。</param>
        /// <returns>返回操作符号。</returns>
        protected virtual string GenerateBinaryOperator(ExpressionType op)
        {
            return _binaryOperatorMap[op];
        }

        /// <summary>
        /// 解析Lambda表达式。
        /// </summary>
        /// <typeparam name="T">表达式类型。</typeparam>
        /// <param name="node">当前表达式实例对象。</param>
        /// <returns>返回解析的表达式。</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var expression = node.Body;
            if (expression.NodeType == ExpressionType.MemberAccess && expression.Type == typeof(bool))
                return Visit(Expression.Equal(expression, Expression.Constant(true)));
            return base.VisitLambda(node);
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            return _builder.ToString();
        }

        #region sql expression visitor
        /// <summary>
        /// 访问IsNull表达式。
        /// </summary>
        /// <param name="isNullExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitIsNull(IsNullExpression isNullExpression)
        {
            Check.NotNull(isNullExpression, nameof(isNullExpression));

            Visit(isNullExpression.Operand);

            _builder.Append(" IS NULL");

            return isNullExpression;
        }

        /// <summary>
        /// 访问IsNotNull表达式。
        /// </summary>
        /// <param name="isNotNullExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitIsNotNull(IsNullExpression isNotNullExpression)
        {
            Check.NotNull(isNotNullExpression, nameof(isNotNullExpression));

            Visit(isNotNullExpression.Operand);

            _builder.Append(" IS NOT NULL");

            return isNotNullExpression;
        }

        /// <summary>
        /// 访问Like表达式。
        /// </summary>
        /// <param name="likeExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitLike(LikeExpression likeExpression)
        {
            Check.NotNull(likeExpression, nameof(likeExpression));

            Visit(likeExpression.Match);
            _builder.Append(" LIKE ");

            Visit(likeExpression.Pattern);
            return likeExpression;
        }

        /// <summary>
        /// 访问文本表达式。
        /// </summary>
        /// <param name="literalExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitLiteral(LiteralExpression literalExpression)
        {
            Check.NotNull(literalExpression, nameof(literalExpression));

            var value = literalExpression.Literal;
            _builder.Append(_sqlHelper.EscapeLiteral(value));

            return literalExpression;
        }

        /// <summary>
        /// 访问文本对比表达式。
        /// </summary>
        /// <param name="stringCompareExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitStringCompare(StringCompareExpression stringCompareExpression)
        {
            Visit(stringCompareExpression.Left);

            _builder.Append(GenerateBinaryOperator(stringCompareExpression.Operator));

            Visit(stringCompareExpression.Right);

            return stringCompareExpression;
        }

        /// <summary>
        /// SQL函数表达式。
        /// </summary>
        /// <param name="sqlFunctionExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            _builder.Append(sqlFunctionExpression.FunctionName);
            _builder.Append("(");

            VisitJoin(sqlFunctionExpression.Arguments.ToList());

            _builder.Append(")");
            return sqlFunctionExpression;
        }

        /// <summary>
        /// 访问IN表达式。
        /// </summary>
        /// <param name="inExpression">别名表达式。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public virtual Expression VisitIn(InExpression inExpression)
        {
            Visit(inExpression.Operand);

            _builder.Append(" IN (");

            VisitJoin(ProcessInValues(inExpression.Values));

            _builder.Append(")");

            return inExpression;
        }

        /// <summary>
        /// 访问不包含。
        /// </summary>
        /// <param name="inExpression">IN表达式。</param>
        /// <returns>返回不存在表达式。</returns>
        public virtual Expression VisitNotIn(InExpression inExpression)
        {
            Visit(inExpression.Operand);

            _builder.Append(" NOT IN (");

            VisitJoin(ProcessInValues(inExpression.Values));

            _builder.Append(")");

            return inExpression;
        }

        /// <summary>
        /// 访问CAST AS表达式。
        /// </summary>
        /// <param name="explicitCastExpression">表达式。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public Expression VisitExplicitCast(ExplicitCastExpression explicitCastExpression)
        {
            _builder.Append("CAST(");

            Visit(explicitCastExpression.Operand);

            _builder.Append(" AS ");

            var typeMapping = _typeMapper.GetMapping(explicitCastExpression.Type);

            if (typeMapping == null)
            {
                throw new InvalidOperationException(string.Format(Resources.UnsupportedType, explicitCastExpression.Type.DisplayName(false)));
            }

            _builder.Append(typeMapping);

            _builder.Append(")");
            return explicitCastExpression;
        }

        private static IReadOnlyList<Expression> ProcessInValues(Expression inValues)
        {
            var expressions = new List<Expression>();
            if (inValues is ConstantExpression inConstant)
            {
                AddInExpressionValues(inConstant.Value, expressions, inValues);
                return expressions;
            }

            if (inValues is NewArrayExpression arrayExpression)
                return arrayExpression.Expressions;

            if (inValues is ListInitExpression listExpression)
            {
                foreach (var initializer in listExpression.Initializers)
                {
                    expressions.Add(initializer.Arguments[0]);
                }
                return expressions;
            }

            if (inValues is MemberExpression memberExpression)
            {
                AddInExpressionValues(memberExpression.Invoke(), expressions, inValues);
                return expressions;
            }

            expressions.Add(inValues);
            return expressions;
        }

        private static void AddInExpressionValues(
            object value, List<Expression> inConstants, Expression expression)
        {
            if (value is IEnumerable valuesEnumerable
                && value.GetType() != typeof(string)
                && value.GetType() != typeof(byte[]))
            {
                inConstants.AddRange(valuesEnumerable.Cast<object>().Select(Expression.Constant));
            }
            else
            {
                inConstants.Add(expression);
            }
        }
        #endregion

        #region helper
        /// <summary>
        /// 当前脚本写入实例。
        /// </summary>
        protected IndentedStringBuilder Sql => _builder;

        private void VisitJoin(
            IReadOnlyList<Expression> expressions, Action<IndentedStringBuilder> joinAction = null)
            => VisitJoin(expressions, e => Visit(e), joinAction);

        private void VisitJoin<T>(
            IReadOnlyList<T> items, Action<T> itemAction, Action<IndentedStringBuilder> joinAction = null)
        {
            joinAction = joinAction ?? (isb => isb.Append(", "));

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    joinAction(_builder);
                }

                itemAction(items[i]);
            }
        }
        #endregion
    }
}