using Mozlite.Mvc.Themes;

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
    }
}
