using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Settings;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Email
{
    [PermissionAuthorize(Permissions.EmailSettings)]
    public class SettingsModel : AdminModelBase
    {
        private readonly ISettingsManager _settingsManager;

        public SettingsModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [BindProperty]
        public EmailSettings Input { get; set; }

        public void OnGet()
        {
            Input = _settingsManager.GetSettings<EmailSettings>();
        }

        public IActionResult OnPost()
        {
            if (Input.Enabled&& !ModelState.IsValid)
                return ErrorPage("更改邮件信息配置失败！");

            if (_settingsManager.SaveSettings(Input))
            {
                Log("修改了邮件配置！");
                return SuccessPage("你已经成功配置了信息！");
            }

            return ErrorPage("更改邮件信息配置失败！");
        }
    }
}