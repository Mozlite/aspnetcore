using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mozlite.Tasks
{
    /// <summary>
    /// 后台服务宿主。
    /// </summary>
    public class TaskHostedService : HostedService
    {
        private readonly IEnumerable<ITaskExecutor> _executors;
        private readonly ILogger _logger;

        /// <summary>
        /// 初始化类<see cref="TaskHostedService"/>。
        /// </summary>
        /// <param name="executors">后台启动注册服务。</param>
        /// <param name="logger">日志接口。</param>
        public TaskHostedService(IEnumerable<ITaskExecutor> executors, ILogger<TaskHostedService> logger)
        {
            _executors = executors;
            _logger = logger;
        }

        /// <summary>
        /// 当应用程序开启时候触发得方法。
        /// </summary>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("后台服务启动！");
            return base.StartAsync(cancellationToken);
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
                foreach (var executor in _executors)
                {
#pragma warning disable 4014
                    Task.Run(async () => await executor.ExecuteAsync(cancellationToken), cancellationToken);
#pragma warning restore 4014
                }
            }
            return Task.CompletedTask;
        }
    }
}