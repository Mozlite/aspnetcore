using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Settings;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Settings
{
    public class EditModel : AdminModelBase
    {
        private readonly ISettingDictionaryManager _settingManager;
        public EditModel(ISettingDictionaryManager settingManager)
        {
            _settingManager = settingManager;
        }

        [BindProperty]
        public SettingDictionary Input { get; set; }

        public IActionResult OnGet(int id, int pid)
        {
            if (id > 0)
            {
                Input = _settingManager.Find(id);
                if (Input == null)
                    return NotFound();
            }
            else
                Input = new SettingDictionary { ParentId = pid };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Input.Name))
            {
                ModelState.AddModelError("Input.Name", "名称不能为空！");
                return Error();
            }

            var result = await _settingManager.SaveAsync(Input);
            if (result)
            {
                await EventLogger.LogCoreResultAsync(result, "字典实例，{0}：{1}。", Input.Name, Input.Value);
            }

            return Json(result, Input.Value);
        }
    }
}