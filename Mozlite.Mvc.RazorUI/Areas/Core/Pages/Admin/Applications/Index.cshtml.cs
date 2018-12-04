using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Apis;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Applications
{
    public class IndexModel : AdminModelBase
    {
        private readonly IApiManager _apiManager;

        public IndexModel(IApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        public IEnumerable<Application> Apps { get; private set; }

        public void OnGet()
        {
            Apps = _apiManager.Load();
        }

        public IActionResult OnPostDelete(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Error("请先选择应用后在进行删除操作！");
            }

            var result = _apiManager.DeleteApplications(ids);
            if (result)
            {
                Log("删除了应用：{0}。", string.Join(",", ids));
            }

            return Json(result, "应用");
        }
    }
}