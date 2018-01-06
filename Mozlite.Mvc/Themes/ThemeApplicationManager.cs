using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Mvc.Themes
{
    /// <summary>
    /// 模板应用程序管理类。
    /// </summary>
    public class ThemeApplicationManager : IThemeApplicationManager
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IReadOnlyCollection<IThemeApplication> _applications;

        /// <summary>
        /// 初始化类<see cref="ThemeApplicationManager"/>。
        /// </summary>
        /// <param name="applications">应用程序列表。</param>
        /// <param name="permissionManager">权限管理接口。</param>
        public ThemeApplicationManager(IEnumerable<IThemeApplication> applications, IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
            applications = applications.OrderByDescending(x => x.Priority).ToList();
            foreach (var application in applications)
            {
                permissionManager.Save(new Permission { Name = $"app.{application.PermissionName}", Description = application.Description });
            }
            permissionManager.RefreshAdministrators();
            _applications = new ReadOnlyCollection<IThemeApplication>(applications.ToList());
        }

        /// <summary>
        /// 获取当前应用程序列表。
        /// </summary>
        /// <param name="mode">显示模式。</param>
        /// <returns>返回当前用户的应用程序列表。</returns>
        public async Task<IEnumerable<IThemeApplication>> LoadApplicationsAsync(NavigateMode mode = NavigateMode.Module)
        {
            var applications = new List<IThemeApplication>();
            foreach (var application in _applications)
            {
                if ((application.Mode & mode) != mode)
                    continue;
                if (await _permissionManager.IsAuthorizedAsync($"app.{application.PermissionName}"))
                    applications.Add(application);
            }
            return applications;
        }

        /// <summary>
        /// 获取当前应用程序列表。
        /// </summary>
        /// <param name="mode">显示模式。</param>
        /// <returns>返回当前用户的应用程序列表。</returns>
        public IEnumerable<IThemeApplication> LoadApplications(NavigateMode mode = NavigateMode.Module)
        {
            var applications = new List<IThemeApplication>();
            foreach (var application in _applications)
            {
                if ((application.Mode & mode) != mode)
                    continue;
                if (_permissionManager.IsAuthorized($"app.{application.PermissionName}"))
                    applications.Add(application);
            }
            return applications;
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="applicationName">应用程序名称。</param>
        /// <returns>返回应用程序实例。</returns>
        public IThemeApplication GetApplication(string applicationName)
        {
            return _applications.SingleOrDefault(x =>
                x.ApplicationName.Equals(applicationName, StringComparison.OrdinalIgnoreCase));
        }
    }
}