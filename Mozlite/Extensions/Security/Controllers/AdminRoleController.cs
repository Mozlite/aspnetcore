using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Extensions.Security.Controllers
{
    /// <summary>
    /// 角色控制器。
    /// </summary>
    public class AdminRoleController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}