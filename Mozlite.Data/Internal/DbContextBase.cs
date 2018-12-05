using Microsoft.Extensions.Logging;
using Mozlite.Data.Query;
using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        public IQueryable<TModel> AsQueryable() => new QueryContext<TModel>(SqlHelper, _visitorFactory, SqlGenerator, _executor);

        /// <summary>
        /// 脚本生成接口。
        /// </summary>
        protected IQuerySqlGenerator SqlGenerator { get; }
        private readonly IDbExecutor _executor;
        private readonly IExpressionVisitorFactory _visitorFactory;

        /// <summary>
        /// 初始化类<see cref="DbContextBase{TModel}"/>。
        /// </summary>
        /// <param name="executor">数据库执行接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">脚本生成器。</param>
        /// <param name="visitorFactory">条件表达式解析器工厂实例。</param>
        protected DbContextBase(IDbExecutor executor, ILogger logger, ISqlHelper sqlHelper, IQuerySqlGenerator sqlGenerator, IExpressionVisitorFactory visitorFactory)
        {
            Logger = logger;
            SqlHelper = sqlHelper;
            SqlGenerator = sqlGenerator;
            _executor = executor;
            _visitorFactory = visitorFactory;
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
        /// 更新所有模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual bool Update(object key, object statement)
        {
            var sql = SqlGenerator.Update(EntityType, statement);
            sql.AddPrimaryKey(key);
            return ExecuteSql(sql);
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
            return ExecuteSql(sql);
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
            return ExecuteSql(sql);
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
            return ExecuteSqlAsync(sql, cancellationToken);
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
            return await ExecuteSqlAsync(sql, cancellationToken);
        }

        /// <summary>
        /// 更新所有的模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="statement">更新选项实例。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回是否更新成功。</returns>
        public virtual Task<bool> UpdateAsync(object key, object statement, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Update(EntityType, statement);
            sql.AddPrimaryKey(key);
            return ExecuteSqlAsync(sql, cancellationToken);
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
            var builder = SqlGenerator.Select(EntityType, expression);
            return Query(builder, builder.Parameters);
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
            var builder = SqlGenerator.Select(EntityType, expression);
            return await QueryAsync(builder, builder.Parameters, cancellationToken);
        }

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual TModel Query(string sql, object parameters = null)
        {
            using (var reader = ExecuteReader(sql, parameters))
            {
                if (reader.Read())
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<TModel> QueryAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            using (var reader = await ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    return EntityType.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 快速构建唯一主键SQL语句。
        /// </summary>
        /// <param name="header">SQL语句头，如：DELETE FROM等。</param>
        /// <param name="key">主键值。</param>
        /// <returns>返回SQL构建实例。</returns>
        protected SqlIndentedStringBuilder PrimaryKeySql(string header, object key)
        {
            return SqlGenerator.PrimaryKeySql(EntityType, header, key);
        }

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual bool Delete(object key)
        {
            return ExecuteSql(PrimaryKeySql("DELETE FROM", key));
        }

        /// <summary>
        /// 根据主键删除模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>判断是否删除成功。</returns>
        public virtual Task<bool> DeleteAsync(object key, CancellationToken cancellationToken = default)
        {
            return ExecuteSqlAsync(PrimaryKeySql("DELETE FROM", key), cancellationToken);
        }

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual TModel Find(object key)
        {
            var sql = PrimaryKeySql("SELECT * FROM", key);
            return Query(sql, sql.Parameters);
        }

        /// <summary>
        /// 通过主键获取模型实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual Task<TModel> FindAsync(object key, CancellationToken cancellationToken = default)
        {
            var sql = PrimaryKeySql("SELECT * FROM", key);
            return QueryAsync(sql, sql.Parameters, cancellationToken);
        }

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            var sql = SqlGenerator.Select(EntityType, expression);
            return Fetch(sql, sql.Parameters);
        }

        /// <summary>
        /// 通过条件表达式获取模型实例对象。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Select(EntityType, expression);
            return await FetchAsync(sql, sql.Parameters, cancellationToken);
        }

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual IEnumerable<TModel> Fetch(string sql, object parameters = null)
        {
            var models = new List<TModel>();
            using (var reader = ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                    models.Add(EntityType.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 通过SQL语句获取模型实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="parameters">参数。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例对象。</returns>
        public virtual async Task<IEnumerable<TModel>> FetchAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            var models = new List<TModel>();
            using (var reader = await ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                    models.Add(EntityType.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="countExpression">返回总记录数的表达式,用于多表拼接过滤重复记录数。</param>
        /// <returns>返回分页实例列表。</returns>
        public TQuery Load<TQuery>(TQuery query, Expression<Func<TModel, object>> countExpression = null) where TQuery : QueryBase<TModel>
        {
            var context = AsQueryable();
            query.Init(context);
            query.Models = context.AsEnumerable(query.PI, query.PS, countExpression);
            return query;
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="countExpression">返回总记录数的表达式,用于多表拼接过滤重复记录数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        public async Task<TQuery> LoadAsync<TQuery>(TQuery query, Expression<Func<TModel, object>> countExpression = null,
            CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>
        {
            var context = AsQueryable();
            query.Init(context);
            query.Models = await context.AsEnumerableAsync(query.PI, query.PS, countExpression, cancellationToken);
            return query;
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
            return await ExecuteScalarAsync(sql, cancellationToken: cancellationToken) != null;
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool Any(object key)
        {
            var sql = SqlGenerator.Any(EntityType);
            sql.AddPrimaryKey(key);
            return ScalarSql(sql) != null;
        }

        /// <summary>
        /// 通过条件表达式判断是否存在实例对象。
        /// </summary>
        /// <param name="key">主键值，主键必须为一列时候才可使用。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> AnyAsync(object key, CancellationToken cancellationToken)
        {
            var sql = SqlGenerator.Any(EntityType);
            sql.AddPrimaryKey(key);
            return await ScalarSqlAsync(sql, cancellationToken) != null;
        }

        /// <summary>
        /// 获取条件表达式的数量。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回计算结果。</returns>
        public virtual int Count(Expression<Predicate<TModel>> expression)
        {
            var sql = SqlGenerator.Scalar(EntityType, "COUNT", null, expression, "1");
            var scalar = ExecuteScalar(sql);
            if (scalar == null || scalar == DBNull.Value)
                return 0;
            return Convert.ToInt32(scalar);
        }

        /// <summary>
        /// 获取条件表达式的数量。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回计算结果。</returns>
        public virtual async Task<int> CountAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Scalar(EntityType, "COUNT", null, expression, "1");
            var scalar = await ExecuteScalarAsync(sql, cancellationToken: cancellationToken);
            if (scalar == null || scalar == DBNull.Value)
                return 0;
            return Convert.ToInt32(scalar);
        }

        /// <summary>
        /// 通过条件表达式获取聚合实例对象。
        /// </summary>
        /// <param name="convertFunc">转换函数。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="scalarMethod">聚合方法。</param>
        /// <returns>返回聚合结果。</returns>
        public virtual TValue GetScalar<TValue>(string scalarMethod, Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression, Func<object, TValue> convertFunc)
        {
            var sql = SqlGenerator.Scalar(EntityType, scalarMethod, column, expression);
            var scalar = ExecuteScalar(sql);
            if (scalar == null || scalar == DBNull.Value)
                return default;
            if (convertFunc == null)
                return (TValue)Convert.ChangeType(scalar, typeof(TValue));
            return convertFunc(scalar);
        }

        /// <summary>
        /// 通过条件表达式获取聚合实例对象。
        /// </summary>
        /// <param name="convertFunc">转换函数。</param>
        /// <param name="column">当前列。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="scalarMethod">聚合方法。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回聚合结果。</returns>
        public virtual async Task<TValue> GetScalarAsync<TValue>(string scalarMethod, Expression<Func<TModel, object>> column, Expression<Predicate<TModel>> expression, Func<object, TValue> convertFunc,
            CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Scalar(EntityType, scalarMethod, column, expression);
            var scalar = await ExecuteScalarAsync(sql, cancellationToken: cancellationToken);
            if (scalar == null || scalar == DBNull.Value)
                return default;
            if (convertFunc == null)
                return (TValue)Convert.ChangeType(scalar, typeof(TValue));
            return convertFunc(scalar);
        }

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveUp(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null)
        {
            var sql = SqlGenerator.Move(EntityType, ">", order, expression);
            sql.AddPrimaryKey(key);
            var scalar = ScalarSql(sql);
            return Convert.ToBoolean(scalar);
        }

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveDown(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null)
        {
            var sql = SqlGenerator.Move(EntityType, "<", order, expression);
            sql.AddPrimaryKey(key);
            var scalar = ScalarSql(sql);
            return Convert.ToBoolean(scalar);
        }

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveUpAsync(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Move(EntityType, ">", order, expression);
            sql.AddPrimaryKey(key);
            var scalar = await ScalarSqlAsync(sql, cancellationToken);
            return Convert.ToBoolean(scalar);
        }

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name="key">主键值。</param>
        /// <param name="order">排序。</param>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveDownAsync(object key, Expression<Func<TModel, object>> order, Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            var sql = SqlGenerator.Move(EntityType, "<", order, expression);
            sql.AddPrimaryKey(key);
            var scalar = await ScalarSqlAsync(sql, cancellationToken);
            return Convert.ToBoolean(scalar);
        }

        #region helpers
        /// <summary>
        /// 查询数据库聚合值。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回执行结果。</returns>
        protected object ScalarSql(SqlIndentedStringBuilder sql)
        {
            return ExecuteScalar(sql, sql.Parameters);
        }

        /// <summary>
        /// 查询数据库聚合值。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回执行结果。</returns>
        protected Task<object> ScalarSqlAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(sql, sql.Parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行没有返回值的查询实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回执行结果。</returns>
        protected bool ExecuteSql(SqlIndentedStringBuilder sql)
        {
            return ExecuteNonQuery(sql, sql.Parameters);
        }

        /// <summary>
        /// 执行没有返回值的查询实例对象。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回执行结果。</returns>
        protected Task<bool> ExecuteSqlAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(sql, sql.Parameters, cancellationToken: cancellationToken);
        }
        #endregion
    }
}