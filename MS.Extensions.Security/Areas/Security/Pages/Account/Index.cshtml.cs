using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages;
using MS.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    public class IndexModel : ModelBase
    {
        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required(ErrorMessage = "{0}不能为空！")]
            [EmailAddress]
            [Display(Name = "电子邮件")]
            public string Email { get; set; }

            [Display(Name = "电话号码")]
            public string PhoneNumber { get; set; }

            [Display(Name = "用户名")]
            [Required(ErrorMessage = "{0}不能为空！")]
            public string UserName { get; set; }
        }

        private readonly IUserManager _userManager;
        private readonly IMessageManager _emailSender;

        public IndexModel(
            IUserManager userManager,
            IMessageManager emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            Input = new InputModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };

            IsEmailConfirmed = user.EmailConfirmed;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            user.UserName = Input.UserName;

            if (Input.Email != user.Email)
            {
                user.Email = Input.Email;
                user.NormalizedEmail = _userManager.NormalizeKey(Input.Email);
                user.EmailConfirmed = false;
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = Input.PhoneNumber;
                user.PhoneNumberConfirmed = false;
            }
            await _userManager.UpdateAsync(user.UserId,
                new
                {
                    user.UserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed
                });

            await _userManager.SignInManager.RefreshSignInAsync(user);
            return RedirectToSuccessPage("你已经成功更新了用户资料。");
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.UserId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.UserId,
                user.Email,
                "确认电子邮件",
                $"请确认激活电子邮件，<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里进行激活</a>.");

            return RedirectToSuccessPage("验证邮件已经发送，请打开邮箱进行验证。");
        }
    }
}
