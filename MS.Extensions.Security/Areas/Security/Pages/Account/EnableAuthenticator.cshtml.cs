using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    public class EnableAuthenticatorModel : ModelBase
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0}不能为空！")]
            [StringLength(7, ErrorMessage = "{0}的长度必须在{2}和{1}之间。", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "验证码")]
            public string Code { get; set; }
        }

        private readonly IUserManager _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public EnableAuthenticatorModel(
            IUserManager userManager,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                NotFound("用户不存在！");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            // Strip spaces and hypens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "验证失败！");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            user.TwoFactorEnabled = true;
            Log("激活二次登陆验证。", user.UserName);

            StatusMessage("你已经成功绑定二次登陆验证。");

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                RecoveryCodes = recoveryCodes.ToArray();
                return RedirectToPage("./ShowRecoveryCodes");
            }

            return RedirectToPage("./TwoFactorAuthentication");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(User user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);

            AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
