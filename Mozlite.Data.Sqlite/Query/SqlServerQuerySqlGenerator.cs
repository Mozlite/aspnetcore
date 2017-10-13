using System;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data.Query;

namespace Mozlite.Data.Sqlite.Query
{
    /// <summary>
    /// Sqlite数据库查询字符串生成器。
    /// </summary>
    public class SqliteQuerySqlGenerator : QuerySqlGenerator
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
        /// 初始化类<see cref="SqliteQuerySqlGenerator"/>。
        /// </summary>
        /// <param name="cache">缓存接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="visitorFactory">表达式工厂接口。</param>
        public SqliteQuerySqlGenerator(IMemoryCache cache, ISqlHelper sqlHelper, IExpressionVisitorFactory visitorFactory)
            : base(cache, sqlHelper, visitorFactory)
        {
        }
    }
}