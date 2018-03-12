using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Query;
using Mozlite.Extensions;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 实体数据库操作基类。
    /// </summary>
    /// <typeparam name="TModel">实体模型。</typeparam>
    public abstract class DbContextBase<TModel> : IDbContextBase<TModel>
    {
        /// <summary>
        /// 日志接口。
        /// </summary>  
        public ILogger Logger { get; }

        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        public ISqlHelper SqlHelper { get; }

        /// <summary>
        /// 脚本生成接口。
        /// </summary>
        protected IQuerySqlGenerator SqlGenerator { get; }
        private readonly IDbExecutor _executor;

        /// <summary>
        /// 初始化类<see cref="DbContextBase{TModel}"/>。
        /// </summary>
        /// <param name="executor">数据库执行接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">脚本生成器。</param>
        protected DbContextBase(IDbExecutor executor, ILogger logger, ISqlHelper sqlHelper, IQuerySqlGenerator sqlGenerator)
        {
            Logger = logger;
            SqlHelper = sqlHelper;
            SqlGenerator = sqlGenerator;
            _executor = executor;
            EntityType = typeof(TModel).GetEntityType();
        }

        /// <summary>
        /// 执行没有返回值的查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回是否有执行影响到数据行。</returns>
        public bool ExecuteNonQuery(string commandText, object parameters = null,
                CommandType commandType = CommandType.Text)
            => _executor.ExecuteNonQuery(commandText, parameters, commandType);

        /// <summary>
        /// 查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回数据库读取实例接口。</returns>
        public DbDataReader ExecuteReader(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
            => _executor.ExecuteReader(commandText, parameters, commandType);

        /// <summary>
        /// 查询数据库聚合值。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回聚合值实例对象。</returns>
        public object ExecuteScalar(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
            => _executor.ExecuteScalar(commandText, parameters, commandType);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回影响的行数。</returns>
        public Task<bool> ExecuteNonQueryAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
            => _executor.ExecuteNonQueryAsync(commandText, parameters, commandType, cancellationToken);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回数据库读取器实例对象。</returns>
        public Task<DbDataReader> ExecuteReaderAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
            => _executor.ExecuteReaderAsync(commandText, parameters, commandType, cancellationToken);

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回单一结果实例对象。</returns>
        public Task<object> ExecuteScalarAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
            => _executor.ExecuteScalarAsync(commandText, parameters, commandType, cancellationToken);

        /// <summary>
        /// 当前实体类型。
        /// </summary>
        public IEntityType EntityType { get; }

        /// <summary>
        /// 新建实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回是否成功新建实例。</returns>
        public virtual bool Create(TModel model)
        {
            var sql = SqlGenerator.Create(EntityType);
            if (EntityType.Identity != null)
            {
                var id = ExecuteScalar(sql, sql.CreateEntityParameters(model));
                if (id != null)
                {
                    if (EntityType.Identity.ClrType == typeof(int))
                        EntityType.Identity.Set(model, Convert.ToInt32(id));
                    else if (EntityType.Identity.ClrType == typeof(long))
                        EntityType.Identity.Set(model, Convert.ToInt64(id));
                    else if (EntityType.Identity.ClrType == typeof(short))
                        EntityType.Identity.Set(model, Convert.ToInt16(id));
                    return true;
                }
                return false;
            }
            return ExecuteNonQuery(sql, sql.CreateEntityParameters(model));
        }

        /// <summary>
        /// 新建实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否成功新建实例。</returns>
        public virtual async Task<bool> CreateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Create(EntityType);
            if (EntityType.Identity != null)
            {
                var id = await ExecuteScalarAsync(sql, sql.CreateEntityParameters(model), cancellationToken: cancellationToken);
                if (id != null)
                {
                    if (EntityType.Identity.ClrType == typeof(int))
                        EntityType.Identity.Set(model, Convert.ToInt32(id));
                    else if (EntityType.Identity.ClrType == typeof(long))
                        EntityType.Identity.Set(model, Convert.ToInt64(id));
                    else if (EntityType.Identity.ClrType == typeof(short))
                        EntityType.Identity.Set(model, Convert.ToInt16(id));
                    return true;
                }
                return false;
            }
            return await ExecuteNonQueryAsync(sql, sql.CreateEntityParameters(model), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 更新模型实例，此实例必须包含自增长ID或主键实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual bool Update(TModel model)
        {
            var sql = SqlGenerator.Update(EntityType);
            return ExecuteNonQuery(sql, sql.CreateEntityParameters(model));
        }

        /// <summary>
        /// 更新模型实例，此实例必须包含自增长ID或主键实例对象。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual async Task<bool> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Update(EntityType);
            return await ExecuteNonQueryAsync(sql, sql.CreateEntityParameters(model), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual bool Update(Expression<Predicate<TModel>> expression, object statement)
        {
            var sql = SqlGenerator.Update(EntityType, expression, statement);
            return ExecuteNonQuery(sql, sql.Parameters);
        }

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例，可以为匿名对象或者参数名为字段得表达式。</param>
        /// <returns>返回是否更新成功。</returns>
        /// <example>
        /// public class T{
        ///     public int Id{get;set;}
        ///     public int Views{get;set;}
        ///     public int DayViews{get;set;}
        /// }
        /// db.Update(x=>x.Id == 1, x=>new{Views = x.Views + 1,DayViews = x.DayViews + 1});
        /// db.Update(x=>x.Id == 1, Views=>Views.Views + 1);//这里参数必须为列名称
        /// </example>
        public virtual bool Update(Expression<Predicate<TModel>> expression, Expression<Func<TModel, object>> statement)
        {
            var sql = SqlGenerator.Update(EntityType, expression, statement);
            return ExecuteNonQuery(sql, sql.Parameters);
        }

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例，可以为匿名对象或者参数名为字段得表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        /// <example>
        /// public class T{
        ///     public int Id{get;set;}
        ///     public int Views{get;set;}
        ///     public int DayViews{get;set;}
        /// }
        /// db.Update(x=>x.Id == 1, x=>new{Views = x.Views + 1,DayViews = x.DayViews + 1});
        /// db.Update(x=>x.Id == 1, Views=>Views.Views + 1);//这里参数必须为列名称
        /// </example>
        public virtual Task<bool> UpdateAsync(Expression<Predicate<TModel>> expression, Expression<Func<TModel, object>> statement, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Update(EntityType, expression, statement);
            return ExecuteNonQueryAsync(sql, sql.Parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 更新所有模型实例对象。
        /// </summary>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual bool Update(object statement)
            => Update(null, statement);

        /// <summary>
        /// 根据条件更新相应的模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual async Task<bool> UpdateAsync(Expression<Predicate<TModel>> expression, object statement, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Update(EntityType, expression, statement);
            return await ExecuteNonQueryAsync(sql, sql.Parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 更新所有的模型实例对象。
        /// </summary>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual Task<bool> UpdateAsync(object statement, CancellationToken cancellationToken = default)
            => UpdateAsync(null, statement, cancellationToken);

        /// <summary>
        /// 根据条件删除模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual bool Delete(Expression<Predicate<TModel>> expression = null)
            => ExecuteNonQuery(SqlGenerator.Delete(EntityType, expression));

        /// <summary>
        /// 根据条件删除模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual Task<bool> DeleteAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
            => ExecuteNonQueryAsync(SqlGenerator.Delete(EntityType, expression), cancellationToken: cancellationToken);

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual TModel Find(Expression<Predicate<TModel>> expression)
        {
            Check.NotNull(expression, nameof(expression));
            return ReadSql(SqlGenerator.Fetch(EntityType, expression));
        }

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<TModel> FindAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            Check.NotNull(expression, nameof(expression));
            return await ReadSqlAsync(SqlGenerator.Fetch(EntityType, expression), cancellationToken);
        }

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual bool Delete(object key)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();
            return ExecuteNonQuery(
                $"DELETE FROM {EntityType.Table} WHERE {SqlHelper.DelimitIdentifier(primaryKey.Name)} = @Key;",
                new { Key = key });
        }

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual Task<bool> DeleteAsync(object key, CancellationToken cancellationToken = default)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();
            return ExecuteNonQueryAsync(
                $"DELETE FROM {EntityType.Table} WHERE {SqlHelper.DelimitIdentifier(primaryKey.Name)} = @Key;",
                new { Key = key }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual TModel Find(object key)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();

            using (var reader = ExecuteReader($"SELECT * FROM {EntityType.Table} WHERE {SqlHelper.DelimitIdentifier(primaryKey.Name)} = @Key;",
                new { Key = key }))
            {
                if (reader.Read())
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<TModel> FindAsync(object key, CancellationToken cancellationToken = default)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();

            using (var reader = await ExecuteReaderAsync($"SELECT * FROM {EntityType.Table} WHERE {SqlHelper.DelimitIdentifier(primaryKey.Name)} = @Key;",
                new { Key = key }, cancellationToken: cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            return LoadSql(SqlGenerator.Fetch(EntityType, expression));
        }

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            return await LoadSqlAsync(SqlGenerator.Fetch(EntityType, expression), cancellationToken);
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool Any(Expression<Predicate<TModel>> expression = null)
        {
            var sql = SqlGenerator.Any(EntityType, expression);
            return ExecuteScalar(sql.ToString()) != null;
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> AnyAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Any(EntityType, expression);
            return await ExecuteScalarAsync(sql.ToString(), cancellationToken: cancellationToken) != null;
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool Any(object key)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();
            return ExecuteScalar($"SELECT 1 FROM {SqlHelper.DelimitIdentifier(EntityType.Table)} WHERE {primaryKey.Name} = @Id;", new { Id = key }) != null;
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> AnyAsync(object key, CancellationToken cancellationToken)
        {
            var primaryKey = EntityType.PrimaryKey.Properties.Single();
            return await ExecuteScalarAsync($"SELECT 1 FROM {SqlHelper.DelimitIdentifier(EntityType.Table)} WHERE {primaryKey.Name} = @Id;", new { Id = key }, cancellationToken: cancellationToken) != null;
        }

        #region helpers
        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回模型实例列表。</returns>
        protected IEnumerable<TModel> LoadSql(SqlIndentedStringBuilder sql)
        {
            var models = new List<TModel>();
            using (var reader = ExecuteReader(sql.ToString(), sql.Parameters))
            {
                while (reader.Read())
                    models.Add(EntityType.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例列表。</returns>
        protected async Task<IEnumerable<TModel>> LoadSqlAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            var models = new List<TModel>();
            using (var reader = await ExecuteReaderAsync(sql.ToString(), sql.Parameters, cancellationToken: cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                    models.Add(EntityType.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回模型实例。</returns>
        protected TModel ReadSql(SqlIndentedStringBuilder sql)
        {
            using (var reader = ExecuteReader(sql, sql.Parameters))
            {
                if (reader.Read())
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 读取模型实例。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例。</returns>
        protected async Task<TModel> ReadSqlAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            using (var reader = await ExecuteReaderAsync(sql, sql.Parameters, cancellationToken: cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }
        #endregion
    }
}