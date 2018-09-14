using Microsoft.AspNetCore.Authorization;

namespace Mozlite.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
