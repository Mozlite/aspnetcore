using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 默认安装程序。
    /// </summary>
    public class DefaultInstaller : IInstaller
    {
        /// <summary>
        /// 安装时候预先执行的接口。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public virtual Task<InstallerStatus> ExecuteAsync()
        {
            return Task.FromResult(InstallerStatus.Success);
        }
    }
}