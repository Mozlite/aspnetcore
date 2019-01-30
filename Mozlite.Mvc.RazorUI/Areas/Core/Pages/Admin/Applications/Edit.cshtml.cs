using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Apis;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Applications
{
    public class EditModel : AdminModelBase
    {
        private readonly IApiManager _apiManager;

        public EditModel(IApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        [BindProperty]
        public Application Input { get; set; }

        public void OnGet(int id)
        {
            Input = _apiManager.Find(id) ?? new Application { UserId = UserId };
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Input.Name))
            {
                ModelState.AddModelError("Input.Name", "名称不能为空！");
                return Error();
            }

            var result = await _apiManager.SaveAsync(Input);
            return LogResult(result, "应用", Input.Name);
        }

        public IActionResult OnPostGeneral()
        {
            return Success(new { AppSecret = Cores.GeneralKey(128) });
        }
    }
}