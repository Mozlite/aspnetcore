using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Tasks
{
    /// <summary>
    /// 任务接口，所有任务都会在后台执行。
    /// </summary>
    public interface ITaskExecutor : IServices
    {
        /// <summary>
        /// 执行的后台任务方法。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回任务实例。</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}