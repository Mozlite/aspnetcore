using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Apis;

namespace Mozlite.Extensions.OpenServices.Areas.OpenServices.Pages.Admin.Apis
{
    public class AppModel : AdminModelBase
    {
        private readonly IApiManager _apiManager;

        public AppModel(IApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        public class InputModel
        {
            /// <summary>
            /// 应用程序Id。
            /// </summary>
            public int AppId { get; set; }

            /// <summary>
            /// API列表。
            /// </summary>
            public int[] APIs { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public Application Application { get; private set; }

        public IActionResult OnGet(int id)
        {
            Application = _apiManager.Find(id);
            if (Application == null)
                return NotFound("应用程序不存在！");
            Input = new InputModel
            {
                AppId = id,
                APIs = _apiManager.LoadApplicationApis(id).Select(x => x.Id).ToArray()
            };
            return Page();
        }

        public IActionResult OnPost()
        {
            var result = _apiManager.AddApis(Input.AppId, Input.APIs);
            if (result)
            {
                var application = _apiManager.Find(Input.AppId);
                if (Input.APIs?.Length > 0)
                {
                    var apis = _apiManager.LoadApplicationApis(Input.AppId)
                        .Select(x => x.Name)
                        .ToArray();
                    EventLogger.Log(ApiSettings.EventType, $"绑定\"{application.Name}\"的API：{string.Join(",", apis)}。");
                }
                else
                    EventLogger.Log(ApiSettings.EventType, "清空了\"{0}\"的API。", application.Name);
            }

            return Json(result, "API");
        }
    }
}