using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Tasks;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据库迁移后台执行实现类。
    /// </summary>
    public class DataMigrationTaskExecutor : ITaskExecutor
    {
        private readonly IDataMigrator _dataMigrator;
        private readonly ILogger _logger;
        /// <summary>
        /// 初始化类<see cref="DataMigrationTaskExecutor"/>。
        /// </summary>
        /// <param name="dataMigrator">数据库迁移实例。</param>
        /// <param name="logger">日志接口。</param>
        public DataMigrationTaskExecutor(IDataMigrator dataMigrator, ILogger<ITaskExecutor> logger)
        {
            _dataMigrator = dataMigrator;
            _logger = logger;
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <returns>返回任务实例。</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Database.IsMigrated) return;
            _logger.LogInformation("数据库迁移开始！");
            await _dataMigrator.MigrateAsync();
            Database.IsMigrated = true;
            _logger.LogInformation("数据库迁移完成！");
        }
    }
}