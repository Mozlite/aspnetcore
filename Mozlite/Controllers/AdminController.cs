using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Mvc.Routing;
using Mozlite.Mvc.Themes;

namespace Mozlite.Controllers
{
    /// <summary>
    /// 管理后台控制器。
    /// </summary>
    [Authorize]
    public class AdminController : Mvc.ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IThemeApplicationManager _applicationManager;

        public AdminController(ILogger<AdminController> logger, IThemeApplicationManager applicationManager)
        {
            _logger = logger;
            _applicationManager = applicationManager;
        }

        [Route(RouteSettings.Dashboard)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Menu()
        {
            return View(await _applicationManager.LoadApplicationsAsync());
        }
    }
}