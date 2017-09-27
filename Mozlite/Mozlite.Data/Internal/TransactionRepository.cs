using Microsoft.Extensions.Logging;
using Mozlite.Data.Query;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库事务操作实现类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public class TransactionRepository<TModel> : RepositoryBase<TModel>, ITransactionRepository<TModel>
    {
        private readonly IDbExecutor _executor;

        /// <summary>
        /// 获取其他模型表格操作实例。
        /// </summary>
        /// <typeparam name="TOther">其他模型类型。</typeparam>
        /// <returns>返回当前事务的模型数据库操作实例。</returns>
        public ITransactionRepository<TOther> As<TOther>()
        {
            return new TransactionRepository<TOther>(_executor, Logger, SqlHelper, SqlGenerator);
        }

        /// <summary>
        /// 初始化类<see cref="RepositoryBase{TModel}"/>。
        /// </summary>
        /// <param name="executor">数据库执行接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">脚本生成器。</param>
        public TransactionRepository(IDbExecutor executor, ILogger logger, ISqlHelper sqlHelper, IQuerySqlGenerator sqlGenerator)
            : base(executor, logger, sqlHelper, sqlGenerator)
        {
            _executor = executor;
        }
    }
}