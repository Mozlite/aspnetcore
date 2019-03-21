using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Settings;

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
            settings.Registrable = Input.Registrable;
            settings.RequiredEmailConfirmed = Input.RequiredEmailConfirmed;
            settings.RequiredPhoneNumberConfirmed = Input.RequiredPhoneNumberConfirmed;
            settings.RequiredTwoFactorEnabled = Input.RequiredTwoFactorEnabled;
            settings.LoginDirection = Input.LoginDirection;
            if (Differ.IsDifference(settings))
            {
                _settingsManager.SaveSettings(settings);
                EventLogger.LogUser($"更新了用户配置信息：{Differ}");
            }

            return RedirectToSuccessPage("你已经成功更新了配置！");
        }
    }
}