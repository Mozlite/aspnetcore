using System;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Extensions.Installers;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 任务服务扩展类。
    /// </summary>
    public static class TaskServiceExtensions
    {
        /// <summary>
        /// 等待到安装初始化完成。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <param name="executor">执行的方法。</param>
        /// <returns>返回当前任务。</returns>
        public static async Task WaitInstalledAsync(this CancellationToken cancellationToken,
            Func<CancellationToken, Task> executor)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Installer.Current == InstallerResult.Success)
                    break;
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                await executor(cancellationToken);
            }
        }
    }
}