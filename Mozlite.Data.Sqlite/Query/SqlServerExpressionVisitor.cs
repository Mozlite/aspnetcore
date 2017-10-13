using System;
using System.Linq.Expressions;
using Mozlite.Data.Migrations;
using Mozlite.Data.Query;
using Mozlite.Data.Query.Expressions;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.Sqlite.Query
{
    /// <summary>
    /// Sqlite表达式解析器。
    /// </summary>
    public class SqliteExpressionVisitor : SqlExpressionVisitor
    {
        /// <summary>
        /// 初始化类<see cref="SqliteExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        public SqliteExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator) 
            : base(sqlHelper, typeMapper, memberTranslator, methodCallTranslator, fragmentTranslator)
        {
        }

        /// <summary>
        /// 初始化类<see cref="SqliteExpressionVisitor"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL操作特殊标识接口。</param>
        /// <param name="typeMapper">类型匹配。</param>
        /// <param name="memberTranslator">字段或属性转换接口。</param>
        /// <param name="methodCallTranslator">方法调用转换接口。</param>
        /// <param name="fragmentTranslator">代码段转换接口。</param>
        /// <param name="schemaFunc">获取前缀代理方法。</param>
        public SqliteExpressionVisitor(ISqlHelper sqlHelper, ITypeMapper typeMapper, IMemberTranslator memberTranslator, IMethodCallTranslator methodCallTranslator, IExpressionFragmentTranslator fragmentTranslator, Func<Type, string> schemaFunc)
            : base(sqlHelper, typeMapper, memberTranslator, methodCallTranslator, fragmentTranslator, schemaFunc)
        {
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