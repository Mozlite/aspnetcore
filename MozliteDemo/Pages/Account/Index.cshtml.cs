using Microsoft.AspNetCore.Authorization;
using MozliteDemo.Extensions;

namespace MozliteDemo.Pages.Account
{
    [Authorize]
    public class IndexModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}