using Mozlite.Extensions;
using Mozlite.Mvc.AdminMenus;

namespace MozliteDemo.Extensions
{
    /// <summary>
    /// 管理菜单。
    /// </summary>
    public class AdminMenuProvider : MenuProvider
    {
        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("dashboard", menu => menu.Texted("控制面板", "fa fa-dashboard", int.MaxValue).Page("/Admin/Index"));

            root.AddMenu("sys", menu => menu
                .AddMenu("settings", it => it.Texted("系统配置").Page("/Admin/Settings").Allow(Permissions.Administrator))
            );
        }
    }
}