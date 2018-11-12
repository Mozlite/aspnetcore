using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages;
using MS.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages
{
    public class ForgotPasswordModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "电子邮件不能为空！")]
            [EmailAddress]
            public string Email { get; set; }
        }

        private readonly UserManager<User> _userManager;
        private readonly IMessageManager _emailSender;

        public ForgotPasswordModel(UserManager<User> userManager, IMessageManager emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/ResetPassword",
                    pageHandler: null,
                    values: new { code, area = SecuritySettings.ExtensionName },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.UserId,
                    Input.Email,
                    "重置密码",
                    $"重置密码链接： <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里</a>...");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
