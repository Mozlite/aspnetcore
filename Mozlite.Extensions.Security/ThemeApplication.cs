using Mozlite.Mvc.Themes;
using Mozlite.Mvc.Themes.Menus;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 应用程序实例。
    /// </summary>
    public class ThemeApplication : ThemeApplicationBase
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public override string ApplicationName => SecuritySettings.ExtensionName;

        /// <summary>
        /// 显示名称。
        /// </summary>
        public override string DisplayName => "用户";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "用户管理应用程序";

        /// <summary>
        /// 图标样式。
        /// </summary>
        public override string IconClass => base.IconClass + "fa-user-o";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("users", menu => menu.Titled("用户管理")
                .AddMenu("index", sub => sub.Titled("用户列表").Iconed("fa-user-o").Hrefed("~/" + ApplicationName))
                .AddMenu("disallow", sub => sub.Titled("禁用名称管理").Hrefed("~/" + ApplicationName+ "/admindisallowname"))
                .AddMenu("logs", sub => sub.Titled("日志管理").Hrefed("~/" + ApplicationName + "/adminlogs"))
                .AddMenu("roles", sub => sub.Titled("用户组管理").Iconed("fa-users").Hrefed("~/" + ApplicationName+"/adminrole"))
                .AddMenu("permissions", sub => sub.Titled("权限管理").Hrefed("~/" + ApplicationName + "/adminpermission"))
            );
        }
    }
}