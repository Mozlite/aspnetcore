using Mozlite.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询构建实现类基类。
    /// </summary>
    public abstract class QuerySqlGenerator : IQuerySqlGenerator
    {
        private readonly IExpressionVisitorFactory _visitorFactory;
        /// <summary>
        /// 唯一主键参数名称。
        /// </summary>
        internal const string PrimaryKeyParameterName = "__primarykey__";
        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        protected ISqlHelper SqlHelper { get; }
        /// <summary>
        /// 唯一主键参数实例。
        /// </summary>
        protected string PrimaryKeyParameter => SqlHelper.Parameterized(PrimaryKeyParameterName);
        /// <summary>
        /// 将唯一主键附加到当前语句构建实例中。
        /// </summary>
        /// <param name="builder">SQL语句构建实例。</param>
        /// <param name="entityType">当前实体类。</param>
        /// <param name="terminated">是否结束语句。</param>
        protected void AppendWherePrimaryKey(SqlIndentedStringBuilder builder, IEntityType entityType, bool terminated = true)
        {
            var primaryKey = SqlHelper.DelimitIdentifier(entityType.SingleKey().Name);
            builder.Append($" WHERE {primaryKey} = {PrimaryKeyParameter}");
            if (terminated)
                builder.AppendLine(SqlHelper.StatementTerminator);
        }

        /// <summary>
        /// 初始化类<see cref="QuerySqlGenerator"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="visitorFactory">表达式工厂接口。</param>
        protected QuerySqlGenerator(ISqlHelper sqlHelper, IExpressionVisitorFactory visitorFactory)
        {
            SqlHelper = sqlHelper;
            _visitorFactory = visitorFactory;
        }

        private readonly ConcurrentDictionary<Type, CacheEntry> _creations = new ConcurrentDictionary<Type, CacheEntry>();
        private readonly ConcurrentDictionary<Type, CacheEntry> _updates = new ConcurrentDictionary<Type, CacheEntry>();
        private class CacheEntry
        {
            public CacheEntry(string sql, List<string> parameters)
            {
                Sql = sql;
                Parameters = parameters;
            }

            public string Sql { get; }

            public List<string> Parameters { get; }
        }

        /// <summary>
        /// 新建实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Create(IEntityType entityType)
        {
            var entry = _creations.GetOrAdd(entityType.ClrType, key =>
            {
                var names = entityType.GetProperties()
                    .Where(property => property.IsCreatable())
                    .Select(property => property.Name)
                    .ToList();
                var builder = new SqlIndentedStringBuilder();
                builder.Append("INSERT INTO");
                builder.Append(" ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
                builder.Append("(").JoinAppend(names.Select(SqlHelper.DelimitIdentifier)).Append(")");
                builder.Append("VALUES(")
                    .JoinAppend(names.Select(SqlHelper.Parameterized))
                    .Append(")").AppendLine(SqlHelper.StatementTerminator);
                if (entityType.Identity != null)
                    builder.Append(SelectIdentity());
                return new CacheEntry(builder.ToString(), names);
            });
            return new SqlIndentedStringBuilder(entry.Sql, entry.Parameters);
        }

        /// <summary>
        /// 更新实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Update(IEntityType entityType)
        {
            var entry = _updates.GetOrAdd(entityType.ClrType, key =>
            {
                var names = entityType.GetProperties()
                    .Where(property => property.IsUpdatable())
                    .Select(property => property.Name)
                    .ToList();
                var builder = new SqlIndentedStringBuilder();
                builder.Append("UPDATE ").Append(SqlHelper.DelimitIdentifier(entityType.Table)).Append(" SET ");
                builder.JoinAppend(names.Select(name => $"{SqlHelper.DelimitIdentifier(name)}={SqlHelper.Parameterized(name)}")).AppendLine();
                if (entityType.PrimaryKey != null)
                {
                    var primaryKeys = entityType.PrimaryKey.Properties
                        .Select(p => p.Name)
                        .ToList();
                    builder.Append("WHERE ")
                        .JoinAppend(
                            primaryKeys.Select(
                                name => $"{SqlHelper.DelimitIdentifier(name)}={SqlHelper.Parameterized(name)}"), " AND ")
                        .Append(SqlHelper.StatementTerminator);
                    names.AddRange(primaryKeys);
                }
                return new CacheEntry(builder.ToString(), names);
            });
            return new SqlIndentedStringBuilder(entry.Sql, entry.Parameters);
        }

        /// <summary>
        /// 通过唯一主键更新实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="parameters">匿名对象。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Update(IEntityType entityType, object parameters)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("UPDATE ").Append(SqlHelper.DelimitIdentifier(entityType.Table)).Append(" SET ");
            builder.CreateObjectParameters(parameters);
            builder.JoinAppend(builder.Parameters.Keys.Select(
                name => $"{SqlHelper.DelimitIdentifier(name)}={SqlHelper.Parameterized(name)}"));
            AppendWherePrimaryKey(builder, entityType);
            return builder;
        }

        /// <summary>
        /// 更新实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="parameters">匿名对象。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Update(IEntityType entityType, Expression expression, object parameters)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("UPDATE ").Append(SqlHelper.DelimitIdentifier(entityType.Table)).Append(" SET ");
            builder.CreateObjectParameters(parameters);
            builder.JoinAppend(builder.Parameters.Keys.Select(
                name => $"{SqlHelper.DelimitIdentifier(name)}={SqlHelper.Parameterized(name)}"));
            builder.AppendEx(Visit(expression), " WHERE {0}").Append(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 更新实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="parameters">匿名对象。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Update(IEntityType entityType, Expression expression, LambdaExpression parameters)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("UPDATE ").Append(SqlHelper.DelimitIdentifier(entityType.Table)).Append(" SET ");
            builder.Append(VisitUpdateExpression(parameters));
            builder.AppendEx(Visit(expression), " WHERE {0}").Append(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 快速构建唯一主键SQL语句。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="sqlHeader">SQL语句头，如：DELETE FROM等。</param>
        /// <param name="key">主键值。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder PrimaryKeySql(IEntityType entityType, string sqlHeader, object key)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append(sqlHeader).Append(" ")
                .Append(SqlHelper.DelimitIdentifier(entityType.Table));
            AppendWherePrimaryKey(builder, entityType);
            builder.AddPrimaryKey(key);
            return builder;
        }

        /// <summary>
        /// 忽略锁（脏查询）。
        /// </summary>
        /// <returns>返回SQL字符串。</returns>
        public virtual string WithNolock()
        {
            return null;
        }

        /// <summary>
        /// 删除实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Delete(IEntityType entityType, Expression expression)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("DELETE FROM ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
            builder.AppendEx(Visit(expression), " WHERE {0}").Append(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 获取自增长的SQL脚本字符串。
        /// </summary>
        /// <returns>自增长的SQL脚本字符串。</returns>
        protected abstract string SelectIdentity();

        /// <summary>
        /// 判断唯一主键关联是否存在。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Any(IEntityType entityType)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("SELECT 1 FROM ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
            AppendWherePrimaryKey(builder, entityType);
            return builder;
        }

        /// <summary>
        /// 移动排序。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="direction">方向："&lt;"下移，"&gt;"上移。</param>
        /// <param name="order">排序列。</param>
        /// <param name="expression">分组条件表达式。</param>
        /// <returns>返回SQL构建实例。</returns>
        public abstract SqlIndentedStringBuilder Move(IEntityType entityType, string direction, LambdaExpression order,
            Expression expression);

        /// <summary>
        /// 聚合函数。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="method">聚合函数。</param>
        /// <param name="column">聚合列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="nullColumn">当<paramref name="column"/>为空的时候，使用的值。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Scalar(IEntityType entityType, string method, LambdaExpression column, Expression expression, string nullColumn = null)
        {
            if (column != null)
                nullColumn = SqlHelper.DelimitIdentifier(column.GetPropertyAccess().Name);
            var builder = new SqlIndentedStringBuilder();
            builder.Append($"SELECT {method}({nullColumn}) FROM {SqlHelper.DelimitIdentifier(entityType.Table)}");
            builder.AppendEx(Visit(expression), " WHERE {0}")
                .AppendLine(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 解析表达式。
        /// </summary>
        /// <param name="expression">表达式实例。</param>
        /// <returns>返回解析的表达式字符串。</returns>
        public virtual string Visit(Expression expression)
        {
            if (expression == null)
                return null;
            var visitor = _visitorFactory.Create();
            visitor.Visit(expression);
            return visitor.ToString();
        }

        /// <summary>
        /// 查询实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Select(IEntityType entityType, Expression expression)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("SELECT * FROM ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
            builder.AppendEx(Visit(expression), " WHERE {0}").Append(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 判断是否存在。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Any(IEntityType entityType, Expression expression)
        {
            var builder = new SqlIndentedStringBuilder();
            builder.Append("SELECT 1 FROM ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
            builder.AppendEx(Visit(expression), " WHERE {0}").Append(SqlHelper.StatementTerminator);
            return builder;
        }

        /// <summary>
        /// 生成实体类型的SQL脚本。
        /// </summary>
        /// <param name="sql">SQL查询实例。</param>
        /// <returns>返回SQL脚本。</returns>
        public SqlIndentedStringBuilder Query(IQuerySql sql)
        {
            var builder = new SqlIndentedStringBuilder();
            if (sql.PageIndex != null)
                PageQuery(sql, builder);
            else if (sql.Size != null)
                SizeQuery(sql, builder);
            else
                Query(sql, builder);
            return builder;
        }

        /// <summary>
        /// 解析更新项目表达式。
        /// </summary>
        /// <param name="expression">更新得表达式。</param>
        /// <returns>返回解析的表达式字符串。</returns>
        private string VisitUpdateExpression(LambdaExpression expression)
        {
            var statements = new List<string>();
            if (expression.Body is NewExpression body)
            {
                for (var i = 0; i < body.Members.Count; i++)
                {
                    var field = SqlHelper.DelimitIdentifier(body.Members[i].Name);
                    field += " = ";
                    field += Visit(body.Arguments[i]);
                    statements.Add(field);
                }
                return string.Join(", ", statements);
            }
            var parameter = SqlHelper.DelimitIdentifier(expression.Parameters[0].Name);
            parameter += " = ";
            parameter += Visit(expression);
            return parameter;
        }

        /// <summary>
        /// 查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="SqlIndentedStringBuilder"/>实例。</param>
        protected abstract void Query(IQuerySql sql, SqlIndentedStringBuilder builder);

        /// <summary>
        /// 分页查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="SqlIndentedStringBuilder"/>实例。</param>
        protected abstract void PageQuery(IQuerySql sql, SqlIndentedStringBuilder builder);

        /// <summary>
        /// 选项特定数量的记录数的查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="SqlIndentedStringBuilder"/>实例。</param>
        protected abstract void SizeQuery(IQuerySql sql, SqlIndentedStringBuilder builder);
    }
}