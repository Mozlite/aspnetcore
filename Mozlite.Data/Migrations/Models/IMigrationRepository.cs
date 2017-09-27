using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Migrations.Operations;

namespace Mozlite.Data.Migrations.Models
{
    /// <summary>
    /// 数据迁移数据库操作接口。
    /// </summary>
    public interface IMigrationRepository
    {
        /// <summary>
        /// 判断是否已经存在迁移表。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        bool EnsureMigrationTableExists();

        /// <summary>
        /// 确保已经存在迁移表。
        /// </summary>
        /// <param name="cancellationToken">异步取消标识。</param>
        Task EnsureMigrationTableExistsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取已经迁移到数据库中的实例。
        /// </summary>
        /// <param name="migrationId">迁移Id。</param>
        /// <returns>返回实例列表。</returns>
        Migration FindMigration(string migrationId);

        /// <summary>
        /// 获取已经迁移到数据库中的实例。
        /// </summary>
        /// <param name="migrationId">迁移Id。</param>
        /// <param name="cancellationToken">异步取消标识。</param>
        /// <returns>返回实例列表。</returns>
        Task<Migration> FindMigrationAsync(string migrationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行迁移命令。
        /// </summary>
        /// <param name="migration">迁移实例。</param>
        /// <param name="operations">迁移命令列表。</param>
        /// <returns>返回执行结果。</returns>
        bool Execute(Migration migration, IReadOnlyList<MigrationOperation> operations);

        /// <summary>
        /// 执行迁移命令。
        /// </summary>
        /// <param name="migration">迁移实例。</param>
        /// <param name="operations">迁移命令列表。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回执行结果。</returns>
        Task<bool> ExecuteAsync(Migration migration, IReadOnlyList<MigrationOperation> operations,
            CancellationToken cancellationToken = default);
    }
}