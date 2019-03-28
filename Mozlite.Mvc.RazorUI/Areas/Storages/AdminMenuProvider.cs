using Mozlite.Extensions;
using Mozlite.Mvc.AdminMenus;

namespace Mozlite.Mvc.RazorUI.Areas.Storages
{
    /// <summary>
    /// 管理菜单。
    /// </summary>
    public class AdminMenuProvider : MenuProvider
    {
        /// <summary>
        /// 区域名称。
        /// </summary>
        public const string AreaName = "Storages";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public override void Init(MenuItem root)
        {
            root.AddMenu("sys", menu => menu
                .AddMenu("storages", it => it.Texted("文件管理").Page("/Admin/Index", area: AreaName).Allow(Permissions.Storages))
            );
        }
    }
}
