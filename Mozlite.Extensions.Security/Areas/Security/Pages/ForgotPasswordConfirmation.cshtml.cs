using Microsoft.AspNetCore.Authorization;

namespace Mozlite.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
