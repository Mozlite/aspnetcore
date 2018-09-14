using Microsoft.AspNetCore.Authorization;

namespace MS.Areas.Security.Pages
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
