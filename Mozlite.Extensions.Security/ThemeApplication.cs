using Mozlite.Mvc.Themes;

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
        public override string IconClass => base.IconClass + "fa-user";
    }
}