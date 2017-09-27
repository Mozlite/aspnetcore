using System;
using System.Linq.Expressions;
using Mozlite.Data.Migrations;
using Mozlite.Data.Query;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;
using Mozlite.Data.SqlServer.Query.Expressions;

namespace Mozlite.Data.SqlServer.Query
{
    /// <summary>
    /// SQLServer表达式解析器。
    /// </summary>
    public class SqlServerExpressionVisitor : SqlExpressionVisitor, ISqlServerExpressionVisitor
    {
        /// <summary>
        /// 初始化类<see cref="SqlServerExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        public SqlServerExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator) 
            : base(sqlHelper, typeMapper, memberTranslator, methodCallTranslator, fragmentTranslator)
        {
        }

        /// <summary>
        /// 初始化类<see cref="SqlServerExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        /// <param name="schemaFunc">获取前缀代理方法。</param>
        public SqlServerExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator, Func<Type, string> schemaFunc)
            : base(sqlHelper, typeMapper, memberTranslator, methodCallTranslator, fragmentTranslator, schemaFunc)
        {
        }

        /// <summary>
        /// 解析日期部分的表达式。
        /// </summary>
        /// <param name="datePartExpression">日期部分的表达式。</param>
        /// <returns>返回表达式实例。</returns>
        public Expression VisitDatePartExpression(DatePartExpression datePartExpression)
        {
            Check.NotNull(datePartExpression, nameof(datePartExpression));

            Sql.Append("DATEPART(")
                .Append(datePartExpression.DatePart)
                .Append(", ");
            Visit(datePartExpression.Argument);
            Sql.Append(")");
            return datePartExpression;
        }

        /// <summary>
        /// SQL函数表达式。
        /// </summary>
        /// <param name="sqlFunctionExpression">表达式实例。</param>
        /// <returns>返回访问后的表达式实例对象。</returns>
        public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.FunctionName.StartsWith("@@", StringComparison.Ordinal))
            {
                Sql.Append(sqlFunctionExpression.FunctionName);
                return sqlFunctionExpression;
            }
            return base.VisitSqlFunction(sqlFunctionExpression);
        }
    }
}