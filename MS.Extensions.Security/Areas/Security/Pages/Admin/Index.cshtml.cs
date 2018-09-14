using Microsoft.AspNetCore.Mvc;

namespace MS.Areas.Security.Pages.Admin
{
    public class IndexModel : ModelBase
    {
        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult Get()
        {
            return Page();
        }
    }
}
