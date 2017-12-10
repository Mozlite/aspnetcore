using System;

namespace Mozlite.Mvc.Themes
{
    /// <summary>
    /// 模板应用程序配置基类。
    /// </summary>
    public abstract class ThemeApplicationBase : IThemeApplication
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public virtual string ApplicationName
        {
            get
            {
                var applicationName = GetType().Namespace;
                var index = applicationName.IndexOf(".Extensions.", StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                {
                    applicationName = applicationName.Substring(index + 12);
                    index = applicationName.IndexOf('.');
                    if (index != -1)
                        applicationName = applicationName.Substring(0, index);
                }
                return applicationName.ToLower();
            }
        }

        /// <summary>
        /// 显示名称。
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// 描述。
        /// </summary>
        public virtual string Description { get; } = null;

        /// <summary>
        /// 样式。
        /// </summary>
        public virtual string CssClass => null;

        /// <summary>
        /// 链接地址。
        /// </summary>
        public virtual string LinkUrl => $"/dashboard/{ApplicationName}";

        /// <summary>
        /// 图标样式。
        /// </summary>
        public virtual string IconClass => "fa ";

        /// <summary>
        /// 优先级，越大越靠前。
        /// </summary>
        public virtual int Priority { get; } = 0;

        /// <summary>
        /// 导航模式。
        /// </summary>
        public virtual NavigateMode Mode { get; } = NavigateMode.Module;
    }
}