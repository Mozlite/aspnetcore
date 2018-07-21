using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Tasks;
using Mozlite.Mvc.Routing;
using Mozlite.Mvc.Themes;
using ControllerBase = Mozlite.Mvc.ControllerBase;

namespace Demo.Controllers
{
    /// <summary>
    /// 管理后台控制器。
    /// </summary>
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IThemeApplicationManager _applicationManager;
        private readonly ITaskManager _taskManager;

        public AdminController(ILogger<AdminController> logger, IThemeApplicationManager applicationManager, ITaskManager taskManager)
        {
            _logger = logger;
            _applicationManager = applicationManager;
            _taskManager = taskManager;
        }

        [Route(RouteSettings.Dashboard)]
        public IActionResult Index()
        {
            return View();
        }

        [Route(RouteSettings.Dashboard + "/editor")]
        public IActionResult Editor()
        {
            return View();
        }

        [Route(RouteSettings.Dashboard + "/tasks")]
        public async Task<IActionResult> Tasks()
        {
            return View(await _taskManager.LoadTasksAsync());
        }

        [Route(RouteSettings.Dashboard + "/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        public async Task<IActionResult> Menu()
        {
            return View(await _applicationManager.LoadApplicationsAsync());
        }
    }
}