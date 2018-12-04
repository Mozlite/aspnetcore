using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Apis;
using System.Linq;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Apis
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
                if (Input.APIs?.Length > 0)
                    Log("将应用{0}绑定到API：{1}。", Input.AppId, string.Join(",", Input.APIs));
                else
                    Log("清空了{0}的API。", Input.AppId);
            }

            return Json(result, "API");
        }
    }
}