using Mozlite.Mvc.AdminMenus;

namespace Mozlite.Extensions.OpenServices
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
            root.AddMenu("app", menu => menu.Texted("开放平台", "fa fa-sitemap")
                .AddMenu("index",
                    it => it.Texted("应用管理").Page("/Admin/Applications/Index", area: OpenServiceSettings.ExtensionName)
                        .Allow(Permissions.Administrator))
                .AddMenu("apis",
                    it => it.Texted("API列表").Page("/Admin/Apis/Index", area: OpenServiceSettings.ExtensionName)
                        .Allow(Permissions.Administrator))
                .AddMenu("code",
                    it => it.Texted("状态编码").Page("/Admin/Apis/Code", area: OpenServiceSettings.ExtensionName)
                        .Allow(Permissions.Administrator))
            );
        }
    }
}
