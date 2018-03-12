﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data.Query;
using Mozlite.Extensions;

namespace Mozlite.Data.SqlServer.Query
{
    /// <summary>
    /// SQLServer数据库查询字符串生成器。
    /// </summary>
    public class SqlServerQuerySqlGenerator : QuerySqlGenerator
    {
        /// <summary>
        /// 获取插入数据后自增长的SQL字符串。
        /// </summary>
        /// <returns>返回自增长获取的SQL字符串。</returns>
        protected override string SelectIdentity()
        {
            return "SELECT SCOPE_IDENTITY();";
        }

        /// <summary>
        /// 移动排序。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <param name="direction">方向。</param>
        /// <param name="order">排序列。</param>
        /// <param name="expression">分组条件表达式。</param>
        /// <returns>返回SQL构建实例。</returns>
        public override SqlIndentedStringBuilder Move(IEntityType entityType, string direction, LambdaExpression order,
            Expression expression)
        {
            var column = SqlHelper.DelimitIdentifier(order.GetPropertyAccess().Name);
            var table = SqlHelper.DelimitIdentifier(entityType.Table);
            var primaryKey = SqlHelper.DelimitIdentifier(entityType.PrimaryKey.Properties.Single().Name);
            var where = Visit(expression);
            var builder = new SqlIndentedStringBuilder();
            builder.AppendLine("DECLARE @CurrentOrder int;");
            builder.AppendLine($"SELECT @CurrentOrder = ISNULL({column}, 0) FROM {table} WHERE {primaryKey} = @Id;");
            builder.AppendLine("DECLARE @AffectId int;");
            builder.AppendLine("DECLARE @AffectOrder int;");
            builder.Append($"SELECT TOP(1) @AffectId = {primaryKey}, @AffectOrder = ISNULL({column}, 0) FROM {table} WHERE {column} {direction} @CurrentOrder")
                .AppendEx(where, " AND {0};").AppendLine();
            builder.AppendLine($@"IF @AffectId IS NOT NULL AND @AffectId > 0 BEGIN
	BEGIN TRANSACTION;
	UPDATE {table} SET {column} = @AffectOrder WHERE {primaryKey} = @Id;
	IF(@@ERROR<>0) BEGIN
		ROLLBACK TRANSACTION;
		SELECT 0;
	END
	UPDATE {table} SET {column} = @CurrentOrder WHERE {primaryKey} = @AffectId;
	IF(@@ERROR<>0) BEGIN
		ROLLBACK TRANSACTION;
		SELECT 0;
	END
	COMMIT TRANSACTION;
	SELECT 1;
END");
            return builder;
        }

        /// <summary>
        /// 查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="IndentedStringBuilder"/>实例。</param>
        protected override void Query(IQuerySql sql, SqlIndentedStringBuilder builder)
        {
            builder.Append("SELECT ");
            if (sql.IsDistinct)
                builder.Append("DISTINCT ");
            builder.Append(sql.FieldSql).Append(" ");
            builder.Append(sql.FromSql).Append(" ");
            builder.Append(sql.WhereSql).Append(" ");
            builder.Append(sql.OrderBySql).Append(SqlHelper.StatementTerminator);
        }

        /// <summary>
        /// 分页查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="SqlIndentedStringBuilder"/>实例。</param>
        protected override void PageQuery(IQuerySql sql, SqlIndentedStringBuilder builder)
        {
            builder.Append("SELECT ");
            if (sql.IsDistinct)
                builder.Append("DISTINCT ");
            builder.Append(sql.FieldSql).Append(" ");
            builder.Append(sql.FromSql).Append(" ");
            builder.Append(sql.WhereSql).Append(" ");
            builder.Append(sql.OrderBySql).Append(" ");

            var size = sql.Size ?? 20;
            builder.Append("OFFSET ")
                .Append(Math.Max((sql.PageIndex.Value - 1) * size, 0))
                .Append(" ROWS FETCH NEXT ")
                .Append(size)
                .AppendLine(" ROWS ONLY;");

            builder.Append("SELECT COUNT(");
            if (sql.IsDistinct && sql.Aggregation != "1")
                builder.Append("DISTINCT ");
            builder.Append(sql.Aggregation);
            builder.Append(")");
            builder.Append(sql.FromSql).Append(" ");
            builder.Append(sql.WhereSql).Append(";");
        }

        /// <summary>
        /// 选项特定数量的记录数的查询脚本。
        /// </summary>
        /// <param name="sql">当前查询实例。</param>
        /// <param name="builder"><see cref="SqlIndentedStringBuilder"/>实例。</param>
        protected override void SizeQuery(IQuerySql sql, SqlIndentedStringBuilder builder)
        {
            builder.Append("SELECT ");
            if (sql.IsDistinct)
                builder.Append("DISTINCT ");
            builder.Append("TOP(").Append(sql.Size).Append(") ");
            builder.Append(sql.FieldSql).Append(" ");
            builder.Append(sql.FromSql).Append(" ");
            builder.Append(sql.WhereSql).Append(" ");
            builder.Append(sql.OrderBySql).Append(SqlHelper.StatementTerminator);
        }

        /// <summary>
        /// 初始化类<see cref="SqlServerQuerySqlGenerator"/>。
        /// </summary>
        /// <param name="cache">缓存接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="visitorFactory">表达式工厂接口。</param>
        public SqlServerQuerySqlGenerator(IMemoryCache cache, ISqlHelper sqlHelper, IExpressionVisitorFactory visitorFactory)
            : base(cache, sqlHelper, visitorFactory)
        {
        }
    }
}