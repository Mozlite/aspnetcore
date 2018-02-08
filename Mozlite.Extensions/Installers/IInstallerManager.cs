using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理接口。
    /// </summary>
    /// <typeparam name="TRegistration">注册码类型。</typeparam>
    public interface IInstallerManager<TRegistration>
        where TRegistration : Registration, new()
    {
        /// <summary>
        /// 是否已经安装。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        Task<InstallerStatus> InstalledAsync();
        
        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveLisenceAsync(TRegistration registration);

        /// <summary>
        /// 获取注册码。
        /// </summary>
        /// <returns>返回注册码实例。</returns>
        Task<TRegistration> GetLisenceAsync();
    }
}