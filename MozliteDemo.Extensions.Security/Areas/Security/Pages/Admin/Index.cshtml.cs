﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin
{
    [PermissionAuthorize(Security.Permissions.Users)]
    public class IndexModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public IndexModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public UserQuery Model { get; private set; }

        public void OnGet(UserQuery query)
        {
            query.MaxRoleLevel = Role.RoleLevel;
            Model = _userManager.Load(query);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == UserId)
                return Error("不能删除自己的账号！");
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                EventLogger.LogUser($"删除了账户{user.UserName}。");
                return Success($"你已经成功删除了账户{user.UserName}!");
            }
            return Error(result.ToErrorString());
        }
    }
}