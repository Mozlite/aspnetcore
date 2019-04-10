using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mozlite;
using MozliteDemo.Extensions.ProjectManager.Milestones;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages.Admin.Milestones
{
    public class IndexModel : ModelBase
    {
        private readonly IMilestoneManager _milestoneManager;

        public IndexModel(IMilestoneManager milestoneManager)
        {
            _milestoneManager = milestoneManager;
        }

        public IEnumerable<Milestone> Milestones { get; private set; }

        public void OnGet(int projectid)
        {
            Milestones = _milestoneManager.Fetch();
            if (projectid > 0)
                Milestones = Milestones.Where(x => x.ProjectId == projectid).ToList();
        }

        public IActionResult OnPost(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Error("请先选择里程碑后再进行删除操作！");
            var result = _milestoneManager.Delete(ids);
            if (result)
                EventLogger.LogPM("删除了项目里程碑：{0}", ids.Join(","));
            return Json(result, "里程碑");
        }
    }
}