using Mozlite.Extensions;
using Mozlite.Mvc.AdminMenus;

namespace Mozlite.Mvc.RazorUI.Areas.Core
{
    /// <summary>
    /// 管理菜单。
    /// </summary>
    public class AdminMenuProvider : MenuProvider
    {
        /// <summary>
        /// 区域名称。
        /// </summary>
        public const string AreaName = "Core";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("sys", menu => menu.Texted("系统管理", "fa fa-cogs")
                .AddMenu("tasks", it => it.Texted("后台服务").Page("/Admin/Tasks/Index", area: AreaName).Allow(Permissions.Administrator))
                .AddMenu("email", it => it.Texted("邮件管理").Page("/Admin/Email/Index", area: AreaName).Allow(Permissions.Email))
                .AddMenu("emailsettings", it => it.Texted("邮件配置").Page("/Admin/Email/Settings", area: AreaName).Allow(Permissions.EmailSettings))
                .AddMenu("notifier", it => it.Texted("通知管理").Page("/Admin/Notifications/Index", area: AreaName).Allow(Permissions.Notifications))
                .AddMenu("dicsettings", it => it.Texted("字典管理").Page("/Admin/Settings/Index", area: AreaName).Allow(Permissions.Administrator))
            );
        }
    }
}
