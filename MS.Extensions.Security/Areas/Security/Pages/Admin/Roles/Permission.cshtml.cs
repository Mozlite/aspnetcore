using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Admin.Roles
{
    [PermissionAuthorize(Security.Permissions.Roles)]
    public class PermissionModel : ModelBase
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IRoleManager _roleManager;

        public PermissionModel(IPermissionManager permissionManager, IRoleManager roleManager)
        {
            _permissionManager = permissionManager;
            _roleManager = roleManager;
        }

        public Role Current { get; private set; }

        public IDictionary<string, List<Permission>> Permissions { get; private set; }

        public async Task OnGetAsync(int id)
        {
            Current = await _roleManager.FindByIdAsync(id);
            var permissions = await _permissionManager.LoadPermissionsAsync();
            Permissions = permissions.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.ToList());
        }

        public async Task<IActionResult> OnPostAsync(int roleId)
        {
            var result = await _permissionManager.SaveAsync(roleId, Request);
            if (result.Succeed())
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                Log($"设置了“{role.Name}”的权限。");
            }
            return Json(result, "权限");
        }
    }
}