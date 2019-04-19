using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Users
{
    public class EditModel : ModelBase
    {
        private readonly IProjectUserManager _projectManager;

        public EditModel(IProjectUserManager projectManager)
        {
            _projectManager = projectManager;
        }

        public class InputModel
        {
            public int[] UserIds { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
            Input = new InputModel { UserIds = _projectManager.Fetch().Select(x => x.Id).ToArray() };
        }

        public IActionResult OnPost()
        {
            var result = _projectManager.SaveUsers(Input.UserIds);
            if (result)
            {
                EventLogger.LogPM("更新用户：{0}", string.Join(",", Input.UserIds));
                return Success("成功更新了项目用户!");
            }
            return Error("更新用户失败,请重试!");
        }
    }
}