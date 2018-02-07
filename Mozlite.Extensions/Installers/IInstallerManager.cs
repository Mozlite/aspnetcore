using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理接口。
    /// </summary>
    public interface IInstallerManager : ISingletonService
    {
        /// <summary>
        /// 是否已经安装。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        Task<InstallerResult> IsInstalledAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 安装程序。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回安装任务。</returns>
        Task SetupAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveLisenceAsync(string registration);

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <typeparam name="TRegistration">注册码类型。</typeparam>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveLisenceAsync<TRegistration>(TRegistration registration);
    }
}