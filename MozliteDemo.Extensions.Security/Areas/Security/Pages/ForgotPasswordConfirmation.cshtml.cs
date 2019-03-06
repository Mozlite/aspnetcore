using Microsoft.AspNetCore.Authorization;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
