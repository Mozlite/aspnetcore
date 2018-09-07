using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Mozlite
{
    /// <summary>
    /// 后台服务基类。
    /// </summary>
    public abstract class HostedService : IHostedService, IServices, IDisposable
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;
        /// <summary>
        /// 当应用程序开启时候触发得方法。
        /// </summary>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        /// <summary>
        /// 当应用程序关闭时候触发得方法。
        /// </summary>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;
            _cts.Cancel();
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 执行得方法，一直执行直到设置取消标记。
        /// </summary>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回当前执行得任务。</returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}