using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozlite;
using MozliteDemo.Extensions.ProjectManager.Milestones;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Milestones
{
    public class IndexModel : ModelBase
    {
        private readonly IMilestoneManager _milestoneManager;
        private readonly IProjectManager _projectManager;

        public IndexModel(IMilestoneManager milestoneManager, IProjectManager projectManager)
        {
            _milestoneManager = milestoneManager;
            _projectManager = projectManager;
        }

        public IEnumerable<Milestone> Milestones { get; private set; }

        public Project Project { get; private set; }

        public IActionResult OnGet(int id)
        {
            Project = _projectManager.Find(id);
            if (Project == null)
                return NotFound();
            Milestones = _milestoneManager.Fetch(x => x.ProjectId == id);
            return Page();
        }

        public IActionResult OnPost(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Error("����ѡ����̱����ٽ���ɾ��������");
            var result = _milestoneManager.Delete(ids);
            if (result)
                EventLogger.LogPM("ɾ������Ŀ��̱���{0}", ids.Join(","));
            return Json(result, "��̱�");
        }
    }
}