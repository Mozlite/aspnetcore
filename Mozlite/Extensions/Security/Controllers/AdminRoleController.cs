using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Data;
using Mozlite.Extensions.Security.Services;

namespace Mozlite.Extensions.Security.Controllers
{
    /// <summary>
    /// 角色控制器。
    /// </summary>
    [Authorize]
    public class AdminRoleController : ControllerBase
    {
        private readonly IRoleManager _roleManager;

        public AdminRoleController(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View(_roleManager.Load());
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _roleManager.FindByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0 || id == int.MaxValue)
                return Error("不能删除系统默认的角色！");
            var result = await _roleManager.DeleteAsync(id);
            if (result.Succeeded)
                return Success("你已经成功删除角色！");
            return Error(result.Errors.FirstOrDefault()?.Description ?? "删除角色错误！");
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(string ids)
        {
            if (ids.SplitToInt32().Any(id => id == 0 || id == int.MaxValue))
                return Error("不能删除系统默认的角色！");
            return Json(await _roleManager.DeleteAsync(ids), "角色");
        }
    }
}