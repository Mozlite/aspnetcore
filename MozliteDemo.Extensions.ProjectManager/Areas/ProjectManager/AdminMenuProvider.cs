using Mozlite.Extensions;
using Mozlite.Mvc.AdminMenus;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager
{
    /// <summary>
    /// 菜单。
    /// </summary>
    public class AdminMenuProvider : MenuProvider
    {
        public override void Init(MenuItem root)
        {
            root.AddMenu("pm", menu => menu.Texted("项目管理", "fa fa-gg").Page("/Admin/Index", area: ProjectSettings.ExtensionName).Allow(Permissions.Administrator)
                .AddMenu("tasks", item => item.Texted("任务列表").Page("/Admin/Index", area: ProjectSettings.ExtensionName).Allow(Permissions.Administrator))
                .AddMenu("projects", item => item.Texted("项目列表").Page("/Admin/Projects/Index", area: ProjectSettings.ExtensionName).Allow(Permissions.Administrator))
                .AddMenu("users", item => item.Texted("用户列表").Page("/Admin/Users/Index", area: ProjectSettings.ExtensionName).Allow(Permissions.Administrator))
            );
        }
    }
}
