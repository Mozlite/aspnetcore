using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Projects
{
    public class IndexModel : ModelBase
    {
        private readonly IProjectManager _projectManager;

        public IndexModel(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public IEnumerable<Project> Projects { get; private set; }

        public void OnGet()
        {
            Projects = _projectManager.Fetch();
        }

        public IActionResult OnPost(int id)
        {
            var project = _projectManager.Find(id);
            if (project == null)
                return Error("项目不存在，删除失败！");
            var result = _projectManager.Delete(id);
            if (result)
                EventLogger.LogPM($"删除了项目：{0}", project.Name);
            return Json(result, project.Name);
        }
    }
}