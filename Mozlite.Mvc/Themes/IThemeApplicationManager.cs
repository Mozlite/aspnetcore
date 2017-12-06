using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Themes
{
    /// <summary>
    /// 模板应用程序管理接口。
    /// </summary>
    public interface IThemeApplicationManager : ISingletonService
    {
        /// <summary>
        /// 获取当前应用程序列表。
        /// </summary>
        /// <returns>返回当前用户的应用程序列表。</returns>
        Task<IEnumerable<IThemeApplication>> LoadApplicationsAsync();
    }
}