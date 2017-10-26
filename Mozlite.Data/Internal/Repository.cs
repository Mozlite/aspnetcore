using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Query;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库操作实现类。
    /// </summary>
    /// <typeparam name="TModel">实体模型。</typeparam>
    public class Repository<TModel> : RepositoryBase<TModel>, IRepository<TModel>
    {
        private readonly IDatabase _db;
        private readonly IExpressionVisitorFactory _visitorFactory;

        /// <summary>
        /// 初始化类<see cref="RepositoryBase{TModel}"/>。
        /// </summary>
        /// <param name="executor">数据库执行接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">脚本生成器。</param>
        /// <param name="visitorFactory">条件表达式解析器工厂实例。</param>
        public Repository(IDatabase executor, ILogger<IDatabase> logger, ISqlHelper sqlHelper, IQuerySqlGenerator sqlGenerator, IExpressionVisitorFactory visitorFactory)
            : base(executor, logger, sqlHelper, sqlGenerator)
        {
            _db = executor;
            _visitorFactory = visitorFactory;
        }

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回事务实例对象。</returns>
        public async Task<bool> BeginTransactionAsync(Func<ITransactionRepository<TModel>, Task<bool>> executor, int timeout = 30, CancellationToken cancellationToken = default)
        {
            return
                await
                    _db.BeginTransactionAsync(
                        async transaction =>
                            await
                                executor(new TransactionRepository<TModel>(transaction, Logger, SqlHelper, SqlGenerator)), timeout, cancellationToken);
        }

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <returns>返回事务实例对象。</returns>
        public bool BeginTransaction(Func<ITransactionRepository<TModel>, bool> executor, int timeout = 30)
        {
            return
                _db.BeginTransaction(
                    transaction =>
                        executor(new TransactionRepository<TModel>(transaction, Logger, SqlHelper, SqlGenerator)), timeout);
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
            query.Models = context.AsEnumerable(query.Page, query.PageSize, countExpression);
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
            query.Models = await context.AsEnumerableAsync(query.Page, query.PageSize, countExpression, cancellationToken);
            return query;
        }

        /// <summary>
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        public IQueryable<TModel> AsQueryable() => new QueryContext<TModel>(SqlHelper, _visitorFactory, SqlGenerator, _db);
    }
}