namespace Mozlite.Mvc.Themes
{
    /// <summary>
    /// 模板应用程序。
    /// </summary>
    public interface IThemeApplication : IServices
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// 显示名称。
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 描述。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 样式。
        /// </summary>
        string CssClass { get; }

        /// <summary>
        /// 链接地址。
        /// </summary>
        string LinkUrl { get; }

        /// <summary>
        /// 图标样式。
        /// </summary>
        string IconClass { get; }

        /// <summary>
        /// 优先级，越大越靠前。
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 导航模式。
        /// </summary>
        NavigateMode Mode { get; }
    }
}