using Microsoft.AspNetCore.Authorization;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
