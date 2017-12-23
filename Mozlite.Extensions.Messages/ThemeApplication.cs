using Mozlite.Mvc.Themes;
using Mozlite.Mvc.Themes.Menus;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 应用程序实例。
    /// </summary>
    public class ThemeApplication : ThemeApplicationBase
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public override string ApplicationName => MessageSettings.ExtensionName;

        /// <summary>
        /// 显示名称。
        /// </summary>
        public override string DisplayName => "邮件";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "用户邮件信息";

        /// <summary>
        /// 图标样式。
        /// </summary>
        public override string IconClass => base.IconClass + "fa-envelope";

        /// <summary>
        /// 导航模式。
        /// </summary>
        public override NavigateMode Mode => NavigateMode.MenuBottom;

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            
        }
    }
}
