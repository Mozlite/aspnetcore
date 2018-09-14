using Microsoft.AspNetCore.Authorization;

namespace Mozlite.Areas.Security.Pages
{
    [AllowAnonymous]
    public class LockoutModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
