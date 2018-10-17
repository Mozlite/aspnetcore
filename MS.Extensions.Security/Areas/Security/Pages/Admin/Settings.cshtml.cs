using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Settings;
using Mozlite.Mvc.Logging;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Admin
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
            var storage = LogContext.Create(settings);
            settings.Registrable = Input.Registrable;
            settings.RequiredEmailConfirmed = Input.RequiredEmailConfirmed;
            settings.RequiredPhoneNumberConfirmed = Input.RequiredPhoneNumberConfirmed;
            settings.RequiredTwoFactorEnabled = Input.RequiredTwoFactorEnabled;
            if (storage.Diff(settings))
            {
                _settingsManager.SaveSettings(settings);
                Log($"更新了用户配置信息：{storage}");
            }

            return RedirectToSuccessPage("你已经成功更新了配置！");
        }
    }
}