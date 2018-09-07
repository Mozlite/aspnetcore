using System;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Migrations;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions
{

    /// <summary>
    /// 数据库迁移后台执行实现类。
    /// </summary>
    public class InstallerHostedService : HostedService
    {
        private static InstallerStatus _current = InstallerStatus.DataMigration;
        private static readonly object _locker = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// 当前安装状态。
        /// </summary>
        public static InstallerStatus Current
        {
            get
            {
                lock (_locker)
                {
                    return _current;
                }
            }
            set
            {
                lock (_locker)
                {
                    _current = value;
                }
            }
        }

        /// <summary>
        /// 初始化类<see cref="InstallerHostedService"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者。</param>
        /// <param name="logger">日志接口。</param>
        public InstallerHostedService(IServiceProvider serviceProvider, ILogger<InstallerHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回任务实例。</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //数据库迁移
            if (Current == InstallerStatus.DataMigration)
            {
                _logger.LogInformation("数据库迁移开始！");
                await _serviceProvider.GetRequiredService<IDataMigrator>().MigrateAsync();
                _logger.LogInformation("数据库迁移完成！");
                Current = InstallerStatus.MigrationCompleted;
            }

            //执行安装接口
            if (Current == InstallerStatus.MigrationCompleted)
            {
                var installManager = _serviceProvider.GetRequiredService<IInstallerManager>();
                if (await installManager.IsNewAsync())
                {
                    _logger.LogInformation("初始化网站！");
                    var installer = _serviceProvider.GetRequiredService<IInstaller>();
                    if (await installer.ExecuteAsync())
                    {
                        await installManager.SaveLisenceAsync(new Registration());
                        Current = InstallerStatus.Initializing; //新站需要初始化。
                        _logger.LogInformation("完成网站初始化！");
                    }
                    else
                    {
                        Current = InstallerStatus.Failured; //失败。
                        _logger.LogInformation("网站初始化失败！");
                    }
                }
                else
                {
                    _logger.LogInformation("启动网站！");
                    Current = InstallerStatus.Success;
                }
            }
        }
    }
}