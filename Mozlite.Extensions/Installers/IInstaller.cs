using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装时候执行的接口。
    /// </summary>
    public interface IInstaller : IService
    {
        /// <summary>
        /// 安装时候预先执行的接口。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        Task<InstallerStatus> ExecuteAsync();
    }
}