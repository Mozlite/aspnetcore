using Microsoft.AspNetCore.Authorization;

namespace MS.Areas.Security.Pages
{
    [AllowAnonymous]
    public class LockoutModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
