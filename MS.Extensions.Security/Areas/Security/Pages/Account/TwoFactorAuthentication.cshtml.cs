using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    public class TwoFactorAuthenticationModel : ModelBase
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        private readonly IUserManager _userManager;

        public TwoFactorAuthenticationModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            Is2faEnabled = user.TwoFactorEnabled;
            IsMachineRemembered = await _userManager.SignInManager.IsTwoFactorClientRememberedAsync(user);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            await _userManager.SignInManager.ForgetTwoFactorClientAsync();
            return RedirectToSuccessPage("已经忘记了登陆状态，下次你需要再进行登陆。");
        }
    }
}
