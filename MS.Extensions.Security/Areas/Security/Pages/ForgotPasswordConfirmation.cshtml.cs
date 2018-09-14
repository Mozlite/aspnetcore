using Microsoft.AspNetCore.Authorization;

namespace MS.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
