using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MozliteDemo.Extensions.ProjectManager.Milestones;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Milestones
{
    public class EditModel : ModelBase
    {
        private readonly IMilestoneManager _milestoneManager;

        public EditModel(IMilestoneManager milestoneManager)
        {
            _milestoneManager = milestoneManager;
        }

        [BindProperty]
        public Milestone Input { get; set; }

        public void OnGet(int id, int pid)
        {
            if (id > 0)
                Input = _milestoneManager.Find(id);
            else
                Input = new Milestone { ProjectId = pid };
        }

        public IActionResult OnPost()
        {
            var valid = true;
            if (string.IsNullOrEmpty(Input.Name))
            {
                valid = false;
                ModelState.AddModelError("Input.Name", "名称不能为空！");
            }
            if (valid)
            {
                Input.UserId = UserId;
                var result = _milestoneManager.Save(Input);
                EventLogger.LogPMResult(result, "里程碑：{0}", string.Join(",", Input.Name));
                return Json(result, Input.Name);
            }

            return Error();
        }
    }
}