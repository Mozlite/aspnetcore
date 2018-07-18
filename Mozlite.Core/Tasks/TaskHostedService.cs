using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozlite.Tasks
{
    /// <summary>
    /// 后台服务宿主。
    /// </summary>
    public class TaskHostedService : HostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化类<see cref="TaskHostedService"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供接口。</param>
        /// <param name="logger">日志接口。</param>
        public TaskHostedService(IServiceProvider serviceProvider, ILogger<TaskHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 当应用程序开启时候触发得方法。
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //延迟执行后台线程
            await Task.Delay(3000, cancellationToken);
            _logger.LogInformation("后台服务启动！");
            await base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 当应用程序关闭时候触发得方法。
        /// </summary>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("后台服务停止！");
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 执行得方法，一直执行直到设置取消标记。
        /// </summary>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回当前执行得任务。</returns>
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var executors = scope.ServiceProvider.GetServices<ITaskExecutor>();
                    foreach (var executor in executors)
                    {
#pragma warning disable 4014
                        Task.Run(async () =>
                        {
                            await executor.ExecuteAsync(cancellationToken);
                        }, cancellationToken);
#pragma warning restore 4014
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}