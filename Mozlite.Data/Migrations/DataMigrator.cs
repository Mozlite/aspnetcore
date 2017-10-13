using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozlite.Data.Migrations.Models;
using Mozlite.Data.Migrations.Operations;
using Mozlite.Data.Properties;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据迁移。
    /// </summary>
    public class DataMigrator : IDataMigrator
    {
        private readonly IMigrationRepository _reposority;

        /// <summary>
        /// 日志接口。
        /// </summary>
        protected ILogger Logger { get; }

        private readonly List<IDataMigration> _migrations;
        private readonly string _provider;

        /// <summary>
        /// 初始化类<see cref="DataMigrator"/>。
        /// </summary>
        /// <param name="migrations">迁移列表。</param>
        /// <param name="options">数据选项。</param>
        /// <param name="reposority">迁移数据库操作接口。</param>
        /// <param name="logger">日志接口。</param>
        public DataMigrator(IEnumerable<IDataMigration> migrations, IOptions<DatabaseOptions> options,
            IMigrationRepository reposority, ILogger<DataMigrator> logger)
        {
            _reposority = reposority;
            Logger = logger;
            _provider = options.Value.Provider;
            _migrations = migrations.Where(m => !m.GetType().GetTypeInfo().IsDefined(typeof(NotMappedAttribute)))
                .OrderByDescending(m => m.Priority)
                .ToList();
        }

        private void InitDataMigration(DataMigration migration)
        {
            migration.Logger = Logger;
        }

        private async Task<IEnumerable<OperationsMigration>> LoadMirationsAsync(IDataMigration instance, int version)
        {
            var type = instance.GetType();
            var migration = instance as DataMigration;
            if (migration != null)
                InitDataMigration(migration);
            var dbMigration = await _reposority.FindMigrationAsync(type.FullName);
            if ((dbMigration == null && version == -1) //卸载
                || (dbMigration != null && dbMigration.Version == version)) //版本已经是设置的版本
                return null;

            if (version == 0) //安装版本设为最大值
                version = int.MaxValue;
            if (dbMigration == null || version > dbMigration.Version)
                return Upgrade(migration, dbMigration, version);
            if (dbMigration.Version > version)
                return Downgrade(migration, dbMigration, version);
            return null;
        }

        private IEnumerable<OperationsMigration> LoadMirations(IDataMigration instance, int version)
        {
            var type = instance.GetType();
            var migration = instance as DataMigration;
            if (migration != null)
                InitDataMigration(migration);
            var dbMigration = _reposority.FindMigration(type.FullName);
            if ((dbMigration == null && version == -1) //卸载
                || (dbMigration != null && dbMigration.Version == version)) //版本已经是设置的版本
                return null;

            if (version == 0) //安装版本设为最大值
                version = int.MaxValue;
            if (dbMigration == null || version > dbMigration.Version)
                return Upgrade(migration, dbMigration, version);
            if (dbMigration.Version > version)
                return Downgrade(migration, dbMigration, version);
            return null;
        }

        private OperationsMigration CreateOperationsMigration(string id, int version, Action<MigrationBuilder> action)
        {
            var builder = new MigrationBuilder(_provider);
            action(builder);
            return new OperationsMigration(builder.Operations, id, version);
        }

        private IEnumerable<OperationsMigration> Upgrade(DataMigration migration, Migration dbMigration, int version)
        {
            var id = migration.GetType().FullName;
            if (dbMigration == null)
                yield return CreateOperationsMigration(id, 1, migration.Create);
            var methods = GetMethods(migration, dbMigration?.Version ?? 0, version);
            foreach (var method in methods)
            {
                yield return CreateOperationsMigration(id, method.Version, method.Method);
            }
        }

        private IEnumerable<OperationsMigration> Downgrade(DataMigration migration, Migration dbMigration, int version)
        {
            var methods = GetMethods(migration, version, dbMigration.Version, false);
            foreach (var method in methods)
            {
                yield return CreateOperationsMigration(dbMigration.Id, method.Version, method.Method);
            }
            if (version == -1)
                yield return CreateOperationsMigration(dbMigration.Id, 0, migration.Destroy);
        }

        private class OperationsMigration : Migration
        {
            public OperationsMigration(IReadOnlyList<MigrationOperation> operations, string id, int version)
            {
                Operations = operations;
                Id = id;
                Version = version;
            }

            public IReadOnlyList<MigrationOperation> Operations { get; }
        }

        private List<MethodInfo> GetMethods(DataMigration migration, int version, int endVersion = 0, bool isUp = true)
        {
            var methodHeader = isUp ? "Up" : "Down";
            var methods = migration
                .GetType()
                .GetRuntimeMethods()
                .Where(
                    m =>
                        m.IsPublic && m.ReturnType == typeof(void) &&
                        m.Name.StartsWith(methodHeader, StringComparison.OrdinalIgnoreCase))
                .Select(
                    m =>
                        new MethodInfo
                        {
                            Version = Convert.ToInt32(m.Name.Substring(methodHeader.Length)) + 1,
                            Method = builder => m.Invoke(migration, new[] { builder })
                        })
                .Where(method => method.Version > version && method.Version < endVersion);
            if (isUp)
                return methods.OrderBy(method => method.Version).ToList();
            return methods.OrderByDescending(method => method.Version).ToList();
        }

        private class MethodInfo
        {
            public int Version { get; set; }
            public Action<MigrationBuilder> Method { get; set; }
        }
        
        /// <summary>
        /// 迁移数据。
        /// </summary>
        /// <param name="version">版本：0表示安装到最新，-1卸载！</param>
        public async Task MigrateAsync(int version = 0)
        {
            await _reposority.EnsureMigrationTableExistsAsync();
            foreach (var migration in _migrations)
            {
                try
                {
                    var operations = await LoadMirationsAsync(migration, version);
                    if (!operations.Any())
                        continue;
                    foreach (var operation in operations)
                    {
                        if (!await _reposority.ExecuteAsync(operation, operation.Operations))
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(999, string.Format(Resources.MigrationError, exception.Message), exception);
#if DEBUG
                    Logger.LogError(exception.StackTrace);
#endif
                    throw;
                }
            }
        }
    }
}