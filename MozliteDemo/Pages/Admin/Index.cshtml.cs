using Microsoft.AspNetCore.Authorization;
using MozliteDemo.Extensions;

namespace MozliteDemo.Pages.Admin
{
    [Authorize]
    public class IndexModel : ModelBase
    {
        public void OnGet()
        {
        }
    }
}