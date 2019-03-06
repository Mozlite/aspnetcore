using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Roles
{
    [PermissionAuthorize(Security.Permissions.Roles)]
    public class PermissionModel : ModelBase
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IRoleManager _roleManager;
        private readonly ICategoryManager _categoryManager;

        public PermissionModel(IPermissionManager permissionManager, IRoleManager roleManager, ICategoryManager categoryManager)
        {
            _permissionManager = permissionManager;
            _roleManager = roleManager;
            _categoryManager = categoryManager;
        }

        public Role Current { get; private set; }

        public IEnumerable<Category> Categories { get; private set; }

        public IDictionary<string, List<Permission>> Permissions { get; private set; }

        public async Task OnGetAsync(int id)
        {
            Current = await _roleManager.FindByIdAsync(id);
            var permissions = await _permissionManager.LoadPermissionsAsync();
            Permissions = permissions.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.ToList());
            Categories = await _categoryManager.FetchAsync(x => !x.Disabled);
        }

        public async Task<IActionResult> OnPostAsync(int roleId)
        {
            var result = await _permissionManager.SaveAsync(roleId, Request);
            if (result.Succeed())
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                EventLogger.LogUser($"设置了“{role.Name}”的权限。");
            }
            return Json(result, "权限");
        }
    }
}