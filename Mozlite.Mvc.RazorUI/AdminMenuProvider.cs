using Mozlite.Extensions;
using Mozlite.Mvc.AdminMenus;

namespace Mozlite.Mvc.RazorUI
{
    /// <summary>
    /// 管理菜单。
    /// </summary>
    public class AdminMenuProvider : MenuProvider
    {
        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("sys", menu => menu.Texted("系统管理", "fa fa-cogs")
                .AddMenu("tasks", it => it.Texted("后台服务").Page("/Admin/Task", area: RazorUISettings.ExtensionName).Allow(Permissions.Administrator))
                .AddMenu("email", it => it.Texted("邮件管理").Page("/Admin/Email/Index", area: RazorUISettings.ExtensionName).Allow(Permissions.Email))
                .AddMenu("emailsettings", it => it.Texted("邮件配置").Page("/Admin/Email/Settings", area: RazorUISettings.ExtensionName).Allow(Permissions.EmailSettings))
                .AddMenu("notifier", it => it.Texted("通知管理").Page("/Admin/Notifications/Index", area: RazorUISettings.ExtensionName).Allow(Permissions.Notifications))
                .AddMenu("storages", it => it.Texted("文件管理").Page("/Admin/Storages/Index", area: RazorUISettings.ExtensionName).Allow(Permissions.Notifications))
            );
        }
    }
}
