using System;
using System.Data;
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
    public class DbContext<TModel> : DbContextBase<TModel>, IDbContext<TModel>
    {
        private readonly IDatabase _db;
        private readonly IExpressionVisitorFactory _visitorFactory;

        /// <summary>
        /// 初始化类<see cref="DbContextBase{TModel}"/>。
        /// </summary>
        /// <param name="executor">数据库执行接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">脚本生成器。</param>
        /// <param name="visitorFactory">条件表达式解析器工厂实例。</param>
        public DbContext(IDatabase executor, ILogger<IDatabase> logger, ISqlHelper sqlHelper, IQuerySqlGenerator sqlGenerator, IExpressionVisitorFactory visitorFactory)
            : base(executor, logger, sqlHelper, sqlGenerator, visitorFactory)
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
        public async Task<bool> BeginTransactionAsync(Func<IDbTransactionContext<TModel>, Task<bool>> executor, int timeout = 30, CancellationToken cancellationToken = default)
        {
            return
                await
                    _db.BeginTransactionAsync(
                        async transaction =>
                            await
                                executor(new DbTransactionContext<TModel>(transaction, Logger, SqlHelper, SqlGenerator, _visitorFactory)), timeout, cancellationToken);
        }

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <returns>返回事务实例对象。</returns>
        public bool BeginTransaction(Func<IDbTransactionContext<TModel>, bool> executor, int timeout = 30)
        {
            return
                _db.BeginTransaction(
                    transaction =>
                        executor(new DbTransactionContext<TModel>(transaction, Logger, SqlHelper, SqlGenerator, _visitorFactory)), timeout);
        }

        /// <summary>
        /// 批量插入数据。
        /// </summary>
        /// <param name="table">模型列表。</param>
        public Task ImportAsync(DataTable table)
        {
            return _db.ImportAsync(table);
        }
    }
}