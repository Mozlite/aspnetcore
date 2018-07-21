using System.Threading.Tasks;
using Demo.Extensions.Security.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Activities;

namespace Demo.Extensions.Security.Controllers
{
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserManager userManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        
        public IActionResult Index(UserQuery query)
        {
            return View(_userManager.Load(query));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == UserId)
                return Warning("不能删除自己的账户！");
            var user = await _userManager.FindByIdAsync(id);
            if (await _userManager.DeleteUserAsync(id))
            {
                _logger.LogUserInformation("删除了用户：{0}(id:{1}, name:{2}).", user.NickName, user.UserId, user.UserName);
                return Success("你已经成功删除了用户！");
            }
            return Error("删除用户失败！");
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(string ids)
        {
            var intIds = ids.SplitToInt32();
            foreach (var id in intIds)
            {
                if (id == UserId)
                    return Warning("不能删除自己的账户！");
            }
            if (await _userManager.DeleteUsersAsync(intIds))
            {
                _logger.Info("删除了用户：{0}.", string.Join(",", intIds));
                return Success("你已经成功删除了所选择的用户！");
            }
            return Error("删除用户失败！");
        }
    }
}