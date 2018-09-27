using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Internal;

namespace Mozlite.Data
{
    /// <summary>
    /// 数据库接口。
    /// </summary>
    public interface IDatabase : IDbExecutor
    {
        /// <summary>
        /// 日志接口。
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回事务实例对象。</returns>
        Task<bool> BeginTransactionAsync(Func<IDbTransaction, Task<bool>> executor, int timeout = 30, CancellationToken cancellationToken = default);

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <returns>返回事务实例对象。</returns>
        bool BeginTransaction(Func<IDbTransaction, bool> executor, int timeout = 30);
        
        /// <summary>
        /// 批量插入数据。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型列表。</param>
        Task BulkInsertAsync<TModel>(IEnumerable<TModel> models);

        /// <summary>
        /// 获取数据库版本信息。
        /// </summary>
        /// <returns>返回数据库版本信息。</returns>
        string GetVersion();
    }
}