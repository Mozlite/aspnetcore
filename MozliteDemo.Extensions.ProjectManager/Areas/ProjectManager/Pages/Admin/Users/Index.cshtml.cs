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
                return Error("����ѡ���û����ٽ���ɾ��������");
            var result = _projectUserManager.Delete(ids);
            if (result)
                EventLogger.LogPM("ɾ������Ŀ�û���{0}", ids.Join(","));
            return Json(result, "�û�");
        }
    }
}