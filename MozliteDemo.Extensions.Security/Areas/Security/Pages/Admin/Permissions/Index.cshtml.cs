using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Permissions
{
    /// <summary>
    /// 权限。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.PermissionManager)]
    public class IndexModel : ModelBase
    {
        private readonly IPermissionManager _permissionManager;
        private readonly ICategoryManager _categoryManager;

        public IndexModel(IPermissionManager permissionManager, ICategoryManager categoryManager)
        {
            _permissionManager = permissionManager;
            _categoryManager = categoryManager;
        }

        public IDictionary<string, List<Permission>> Permissions { get; private set; }

        public IEnumerable<Category> Categories { get; private set; }

        public async Task OnGetAsync()
        {
            var permissions = await _permissionManager.LoadPermissionsAsync();
            Permissions = permissions.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.ToList());
            Categories = await _categoryManager.FetchAsync(x => !x.Disabled);
        }

        public async Task<IActionResult> OnPostMoveUpAsync(int id, string category)
        {
            if (await _permissionManager.MoveUpAsync(id, category))
            {
                var permission = await _permissionManager.GetPermissionAsync(id);
                EventLogger.LogUser($"上移了权限“{permission.Text}”的位置！");
                return Success();
            }
            return Error("上移权限失败！");
        }

        public async Task<IActionResult> OnPostMoveDownAsync(int id, string category)
        {
            if (await _permissionManager.MoveDownAsync(id, category))
            {
                var permission = await _permissionManager.GetPermissionAsync(id);
                EventLogger.LogUser($"下移了权限“{permission.Text}”的位置！");
                return Success();
            }
            return Error("下移权限失败！");
        }
    }
}