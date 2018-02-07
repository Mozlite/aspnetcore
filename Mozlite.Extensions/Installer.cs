using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Migrations;
using Mozlite.Extensions.Installers;
using Mozlite.Tasks;

namespace Mozlite.Extensions
{

    /// <summary>
    /// 数据库迁移后台执行实现类。
    /// </summary>
    public class Installer : ITaskExecutor
    {
        /// <summary>
        /// 当前安装状态。
        /// </summary>
        public static InstallerResult Current { get; internal set; } = InstallerResult.New;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        /// <summary>
        /// 初始化类<see cref="Installer"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者。</param>
        /// <param name="logger">日志接口。</param>
        public Installer(IServiceProvider serviceProvider, ILogger<ITaskExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回任务实例。</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //数据库迁移
            if (Current == InstallerResult.New)
            {
                _logger.LogInformation("数据库迁移开始！");
                await _serviceProvider.GetRequiredService<IDataMigrator>().MigrateAsync();
                _logger.LogInformation("数据库迁移完成！");
            }
            //安装初始化
            var installerManager = _serviceProvider.GetService<IInstallerManager>();
            if (installerManager != null)
            {
                Current = await installerManager.IsInstalledAsync(cancellationToken);
                if (Current == InstallerResult.New)
                    await installerManager.SetupAsync(cancellationToken);
            }
            else
                Current = InstallerResult.Success;
        }
    }
}