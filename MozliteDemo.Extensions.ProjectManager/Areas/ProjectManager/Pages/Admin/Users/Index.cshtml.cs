using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozlite;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Users
{
    public class IndexModel : ModelBase
    {
        private readonly IProjectUserManager _projectUserManager;

        public IndexModel(IProjectUserManager projectUserManager)
        {
            _projectUserManager = projectUserManager;
        }

        public IEnumerable<ProjectUser> Users { get; private set; }

        public void OnGet()
        {
            Users = _projectUserManager.Fetch();
        }

        public IActionResult OnPost(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Error("请先选择用户后再进行删除操作！");
            var result = _projectUserManager.Delete(ids);
            if (result)
                EventLogger.LogPM("删除了项目用户：{0}", ids.Join(","));
            return Json(result, "用户");
        }
    }
}