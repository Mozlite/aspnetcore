using System;

namespace Mozlite.Mvc.Themes
{
    /// <summary>
    /// 导航模式。
    /// </summary>
    [Flags]
    public enum NavigateMode
    {
        /// <summary>
        /// 无。
        /// </summary>
        None = 0,

        /// <summary>
        /// 显示在模块中。
        /// </summary>
        Module = 1,

        /// <summary>
        /// 显示在导航顶部。
        /// </summary>
        MenuTop = 2,

        /// <summary>
        /// 显示在导航底部。
        /// </summary>
        MenuBottom = 4,
    }
}