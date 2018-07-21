using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Extensions.Security.Controllers
{
    /// <summary>
    /// 用户组控制器。
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
                return Error("不能删除系统默认的用户组！");
            var result = await _roleManager.DeleteAsync(id);
            if (result.Succeeded)
                return Success("你已经成功删除用户组！");
            return Error(result.Errors.FirstOrDefault()?.Description ?? "删除用户组错误！");
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(string ids)
        {
            if (ids.SplitToInt32().Any(id => id == 0 || id == int.MaxValue))
                return Error("不能删除系统默认的用户组！");
            return Json(await _roleManager.DeleteAsync(ids), "用户组");
        }
    }
}