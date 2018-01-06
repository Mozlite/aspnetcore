using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Mvc.Routing;
using ControllerBase = Mozlite.Mvc.ControllerBase;

namespace Mozlite.Controllers
{
    [Authorize]
    public class DesignerController : ControllerBase
    {
        [Route(RouteSettings.Dashboard + "/designer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}