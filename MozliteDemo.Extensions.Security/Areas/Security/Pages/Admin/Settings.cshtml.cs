using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Storages;
using System.Threading.Tasks;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin
{
    public class SettingsModel : ModelBase
    {
        private readonly ISettingsManager _settingsManager;

        public SettingsModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [BindProperty]
        public SecuritySettings Input { get; set; }

        public void OnGet()
        {
            Input = _settingsManager.GetSettings<SecuritySettings>();
        }

        public IActionResult OnPost()
        {
            var settings = _settingsManager.GetSettings<SecuritySettings>();
            Differ.Init(settings);
            if (Differ.IsDifference(Input))
            {
                _settingsManager.SaveSettings(Input);
                EventLogger.LogUser($"更新了用户配置信息：{Differ}");
            }

            return RedirectToSuccessPage("你已经成功更新了配置！");
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
        {
            var mediaDirectory = GetRequiredService<IMediaDirectory>();
            var result = await mediaDirectory.UploadAsync(file, SecuritySettings.ExtensionName);
            return Json(result);
        }
    }
}