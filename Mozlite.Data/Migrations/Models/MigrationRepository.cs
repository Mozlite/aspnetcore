using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Data.Migrations.Operations;
using Mozlite.Extensions;

namespace Mozlite.Data.Migrations.Models
{
    /// <summary>
    /// 数据库迁移操作基类。
    /// </summary>
    public abstract class MigrationRepository : IMigrationRepository
    {
        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        protected ISqlHelper SqlHelper { get; }
        /// <summary>
        /// 数据库操作上下文。
        /// </summary>
        protected IDbContext<Migration> Context{get;}
        /// <summary>
        /// SQL脚本生成接口。
        /// </summary>
        protected IMigrationsSqlGenerator SqlGenerator { get; }
        
        /// <summary>
        /// 初始化类<see cref="MigrationRepository"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="sqlGenerator">SQL迁移脚本生成接口。</param>
        protected MigrationRepository(IDbContext<Migration> context, ISqlHelper sqlHelper, IMigrationsSqlGenerator sqlGenerator)
        {
            SqlHelper = sqlHelper;
            Table = typeof(Migration).GetTableName();
            Context = context;
            SqlGenerator = sqlGenerator;
        }

        /// <summary>
        /// 表格名称。
        /// </summary>
        public string Table { get; }

        /// <summary>
        /// 判断是否存在的脚本。
        /// </summary>
        protected abstract string ExistsSql { get; }

        /// <summary>
        /// 主键名称。
        /// </summary>
        protected string PrimaryKeyName => OperationHelper.GetName(NameType.PrimaryKey, Table);

        /// <summary>
        /// 判断是否已经存在迁移表。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public virtual bool EnsureMigrationTableExists()
        {
            if (Context.ExecuteScalar(ExistsSql) == DBNull.Value)
            {
                return Context.ExecuteNonQuery(CreateSql);
            }
            return true;
        }

        /// <summary>
        /// 创建表格语句。
        /// </summary>
        protected abstract string CreateSql { get; }

        /// <summary>
        /// 确保已经存在迁移表。
        /// </summary>
        /// <param name="cancellationToken">异步取消标识。</param>
        public virtual async Task EnsureMigrationTableExistsAsync(CancellationToken cancellationToken = default)
        {
            if (await Context.ExecuteScalarAsync(ExistsSql, cancellationToken: cancellationToken) == DBNull.Value)
            {
                await Context.ExecuteNonQueryAsync(CreateSql, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// 获取已经迁移到数据库中的实例。
        /// </summary>
        /// <param name="migrationId">迁移Id。</param>
        /// <returns>返回实例列表。</returns>
        public Migration FindMigration(string migrationId)
        {
            return Context.Find(m => m.Id == migrationId);
        }

        /// <summary>
        /// 获取已经迁移到数据库中的实例。
        /// </summary>
        /// <param name="migrationId">迁移Id。</param>
        /// <param name="cancellationToken">异步取消标识。</param>
        /// <returns>返回实例列表。</returns>
        public Task<Migration> FindMigrationAsync(string migrationId, CancellationToken cancellationToken = default)
        {
            return Context.FindAsync(m => m.Id == migrationId, cancellationToken);
        }

        /// <summary>
        /// 执行迁移命令。
        /// </summary>
        /// <param name="migration">迁移实例。</param>
        /// <param name="operations">迁移命令列表。</param>
        /// <returns>返回执行结果。</returns>
        public bool Execute(Migration migration, IReadOnlyList<MigrationOperation> operations)
        {
            var commandTexts = SqlGenerator.Generate(operations);
            return Context.BeginTransaction(db =>
            {
                foreach (var commandText in commandTexts)
                {
                    if (!db.ExecuteNonQuery(commandText))
                        return false;
                }
                if (migration.Version == 0)
                    return db.Delete(m => m.Id == migration.Id);
                if (migration.Version == 1 && !db.Any(m => m.Id == migration.Id))
                    return db.Create(migration);
                return db.Update(migration);
            }, 60);
        }

        /// <summary>
        /// 执行迁移命令。
        /// </summary>
        /// <param name="migration">迁移实例。</param>
        /// <param name="operations">迁移命令列表。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回执行结果。</returns>
        public async Task<bool> ExecuteAsync(Migration migration, IReadOnlyList<MigrationOperation> operations,
            CancellationToken cancellationToken = default)
        {
            var commandTexts = SqlGenerator.Generate(operations);
            return await Context.BeginTransactionAsync(async db =>
            {
                foreach (var commandText in commandTexts)
                {
                    await db.ExecuteNonQueryAsync(commandText, cancellationToken: cancellationToken);
                }
                if (migration.Version == 0)
                    return await db.DeleteAsync(m => m.Id == migration.Id, cancellationToken);
                if (migration.Version == 1 && !await db.AnyAsync(m => m.Id == migration.Id, cancellationToken))
                    return await db.CreateAsync(migration, cancellationToken);
                return await db.UpdateAsync(migration, cancellationToken);
            }, 600, cancellationToken);
        }
    }
}