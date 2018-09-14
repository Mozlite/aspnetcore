using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Activities;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages
{
    [AllowAnonymous]
    public class LogoutModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public void OnGet()
        {
        }
        
        public LogoutModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _userManager.SignOutAsync();
            Logger.Info("退出登陆。");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToPage();
        }
    }
}
