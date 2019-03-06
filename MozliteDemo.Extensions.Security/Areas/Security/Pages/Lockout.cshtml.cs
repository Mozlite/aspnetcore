using Microsoft.AspNetCore.Authorization;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages
{
    [AllowAnonymous]
    public class LockoutModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}
