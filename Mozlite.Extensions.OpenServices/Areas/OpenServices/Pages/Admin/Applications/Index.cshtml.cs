using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Apis;

namespace Mozlite.Extensions.OpenServices.Areas.OpenServices.Pages.Admin.Applications
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

            var applications = _apiManager.Load(x => ids.Contains(x.Id))
                .Select(x => x.Name)
                .ToArray();
            var result = _apiManager.DeleteApplications(ids);
            if (result)
            {
                EventLogger.Log(ApiSettings.EventType, "删除了应用：{0}。", string.Join(",", applications));
            }

            return Json(result, "应用");
        }
    }
}