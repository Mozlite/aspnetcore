using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Migrations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 数据库迁移后台执行实现类。
    /// </summary>
    public class InstallerHostedService : HostedService
    {
        private static InstallerStatus _current;
        private static readonly object _locker = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly IInstallerManager _installerManager;
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
        /// <param name="installerManager">安装管理接口。</param>
        /// <param name="logger">日志接口。</param>
        public InstallerHostedService(IServiceProvider serviceProvider, IInstallerManager installerManager, ILogger<InstallerHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _installerManager = installerManager;
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
            _logger.LogInformation("数据库迁移开始...");
            await _serviceProvider.GetRequiredService<IDataMigrator>().MigrateAsync();
            _logger.LogInformation("数据库迁移完成。");

            //启动网站
            _logger.LogInformation("启动网站...");
            var registration = await _installerManager.GetRegistrationAsync();
            if (registration.Expired < DateTimeOffset.Now)
                registration.Status = InstallerStatus.Expired;

            if (registration.Status == InstallerStatus.Initializing)
            {
                try
                {
                    using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        registration.Status = await scope.ServiceProvider.GetRequiredService<IInstaller>().ExecuteAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "网站初始化失败！");
                }
            }
            await _installerManager.SaveRegistrationAsync(registration);
            Current = registration.Status;
            if (Current == InstallerStatus.Failured)
                _logger.LogInformation("启动网站失败。");
            else
                _logger.LogInformation("启动网站完成。");
        }
    }
}