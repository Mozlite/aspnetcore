using Mozlite.Mvc.Routing;
using Mozlite.Mvc.Themes;
using Mozlite.Mvc.Themes.Menus;

namespace Mozlite
{
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
        /// 显示名称。
        /// </summary>
        public override string DisplayName => "画图";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "HTML5画图编辑器";

        /// <summary>
        /// 图标样式。
        /// </summary>
        public override string IconClass => base.IconClass + "fa-free-code-camp";

        /// <summary>
        /// 链接地址。
        /// </summary>
        public override string LinkUrl => $"/{RouteSettings.Dashboard}/editor";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            
        }
    }
}