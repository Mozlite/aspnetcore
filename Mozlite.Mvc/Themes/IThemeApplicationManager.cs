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
        /// <param name="mode">显示模式。</param>
        /// <returns>返回当前用户的应用程序列表。</returns>
        Task<IEnumerable<IThemeApplication>> LoadApplicationsAsync(NavigateMode mode = NavigateMode.Module);

        /// <summary>
        /// 获取当前应用程序列表。
        /// </summary>
        /// <param name="mode">显示模式。</param>
        /// <returns>返回当前用户的应用程序列表。</returns>
        IEnumerable<IThemeApplication> LoadApplications(NavigateMode mode = NavigateMode.Module);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="applicationName">应用程序名称。</param>
        /// <returns>返回应用程序实例。</returns>
        IThemeApplication GetApplication(string applicationName);
    }
}