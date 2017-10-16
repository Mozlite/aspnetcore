using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Extensions;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询构建实现类基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class QuerySqlGenerator : IQuerySqlGenerator
    {
        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        protected ISqlHelper SqlHelper { get; }
        private readonly IMemoryCache _cache;
        private readonly IExpressionVisitorFactory _visitorFactory;

        /// <summary>
        /// 初始化类<see cref="SqlGenerator"/>。
        /// </summary>
        /// <param name="cache">缓存接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="visitorFactory">表达式工厂接口。</param>
        protected QuerySqlGenerator(IMemoryCache cache, ISqlHelper sqlHelper, IExpressionVisitorFactory visitorFactory)
        {
            SqlHelper = sqlHelper;
            _cache = cache;
            _visitorFactory = visitorFactory;
        }

        /// <summary>
        /// 从缓存中获取<see cref="SqlIndentedStringBuilder"/>实例。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="action">操作SQL语句。</param>
        /// <returns>返回SQL构建实例。</returns>
        protected SqlIndentedStringBuilder GetOrCreate(IEntityType entityType, string key, Action<SqlIndentedStringBuilder> action)
        {
            return _cache.GetOrCreate(new Tuple<Type, string>(entityType.ClrType, key), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var builder = new SqlIndentedStringBuilder();
                action(builder);
                return builder;
            });
        }

        /// <summary>
        /// 新建实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Create(IEntityType entityType) => GetOrCreate(entityType, nameof(Create), builder =>
         {
             var names = entityType.GetProperties()
                 .Where(property => !property.IsIdentity)
                 .Select(property => property.Name)
                 .ToList();
             builder.Append("INSERT INTO");
             builder.Append(" ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
             builder.Append("(").JoinAppend(names.Select(SqlHelper.DelimitIdentifier)).Append(")");
             builder.Append("VALUES(")
                 .JoinAppend(names.Select(SqlHelper.Parameterized))
                 .Append(")").AppendLine(SqlHelper.StatementTerminator);
             if (entityType.Identity != null)
                 builder.Append(SelectIdentity());
             builder.AddParameters(names);
         });

        /// <summary>
        /// 更新实例。
        /// </summary>
        /// <param name="entityType">模型实例。</param>
        /// <returns>返回SQL构建实例。</returns>
        public virtual SqlIndentedStringBuilder Update(IEntityType entityType) => GetOrCreate(entityType, nameof(Update), builder =>
          {
              var names = entityType.GetProperties()
                  .Where(property => !property.IsIdentity && !property.PropertyInfo.IsDefined(typeof(NotUpdatedAttribute)))
                  .Select(property => property.Name)
                  .ToList();
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
                              name => $"{SqlHelper.DelimitIdentifier(name)}={SqlHelper.Parameterized(name)}"))
                      .Append(SqlHelper.StatementTerminator);
                  names.AddRange(primaryKeys);
              }
              builder.AddParameters(names);
          });

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
        public virtual SqlIndentedStringBuilder Fetch(IEntityType entityType, Expression expression)
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
            builder.Append("SELECT TOP(1) 1 FROM ").Append(SqlHelper.DelimitIdentifier(entityType.Table));
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