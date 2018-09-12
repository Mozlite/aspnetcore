using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理接口。
    /// </summary>
    public interface IInstallerManager : ISingletonService
    {
        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveLisenceAsync(Registration registration);

        /// <summary>
        /// 获取注册码。
        /// </summary>
        /// <returns>返回注册码实例。</returns>
        Task<Registration> GetLisenceAsync();
    }
}