using Microsoft.AspNetCore.Mvc;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Projects
{
    public class EditModel : ModelBase
    {
        private readonly IProjectManager _projectManager;

        public EditModel(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        [BindProperty]
        public Project Input { get; set; }

        public IActionResult OnGet(int id)
        {
            if (id > 0)
            {
                Input = _projectManager.Find(id);
                if (Input == null)
                    return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            var valid = true;
            if (string.IsNullOrEmpty(Input.Name))
            {
                valid = false;
                ModelState.AddModelError("Input.Name", "项目名称不能为空！");
            }
            if (valid)
            {
                var result = _projectManager.Save(Input);
                EventLogger.LogPMResult(result, "项目：{0}", Input.Name);
                return Json(result, Input.Name);
            }
            return Error();
        }
    }
}