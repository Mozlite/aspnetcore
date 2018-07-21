using Mozlite.Mvc.Routing;
using Mozlite.Mvc.Themes;
using Mozlite.Mvc.Themes.Menus;

namespace Demo
{
    ///// <summary>
    ///// 应用程序配置。
    ///// </summary>
    //public class ThemeApplication : ThemeApplicationBase
    //{
    //    /// <summary>
    //    /// 应用程序名称。
    //    /// </summary>
    //    public override string ApplicationName => null;

    //    /// <summary>
    //    /// 应用程序名称。
    //    /// </summary>
    //    public override string PermissionName => "designer";

    //    /// <summary>
    //    /// 显示名称。
    //    /// </summary>
    //    public override string DisplayName => "画图";

    //    /// <summary>
    //    /// 描述。
    //    /// </summary>
    //    public override string Description => "HTML5画图编辑器";

    //    /// <summary>
    //    /// 图标样式。
    //    /// </summary>
    //    public override string IconClass => base.IconClass + "fa-free-code-camp";

    //    /// <summary>
    //    /// 链接地址。
    //    /// </summary>
    //    public override string LinkUrl => $"/{RouteSettings.Dashboard}/designer";

    //    /// <summary>
    //    /// 初始化菜单实例。
    //    /// </summary>
    //    /// <param name="root">根目录菜单。</param>
    //    public override void Init(MenuItem root)
    //    {

    //    }
    //}

    /// <summary>
    /// 应用程序配置。
    /// </summary>
    public class ThemeApplication : ThemeApplicationBase
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public override string ApplicationName => null;

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public override string PermissionName => "settings";

        /// <summary>
        /// 显示名称。
        /// </summary>
        public override string DisplayName => "设置";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "网站设置";

        /// <summary>
        /// 图标样式。
        /// </summary>
        public override string IconClass => base.IconClass + "fa-cog";

        /// <summary>
        /// 链接地址。
        /// </summary>
        public override string LinkUrl => $"/{RouteSettings.Dashboard}/settings";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("settings", menu => menu.Titled("网站设置")
                .AddMenu("home", item => item.Titled("网站配置").Iconed("fa-cog").Hrefed($"/{RouteSettings.Dashboard}/settings"))
                .AddMenu("tasks", item => item.Titled("后台服务").Iconed("fa-tasks").Hrefed($"/{RouteSettings.Dashboard}/tasks"))
            );
        }
    }
}