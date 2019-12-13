using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Roles
{
    [PermissionAuthorize(Security.Permissions.Roles)]
    public class EditModel : ModelBase
    {
        private readonly IRoleManager _roleManager;

        public EditModel(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int? RoleId { get; set; }

            [Required(ErrorMessage = "角色名称不能为空！")]
            public string Name { get; set; }

            [Required(ErrorMessage = "角色唯一键不能为空！")]
            public string NormalizedName { get; set; }
        }

        public void OnGet(int id)
        {
            var role = _roleManager.FindById(id);
            if (role != null)
                Input = new InputModel { Name = role.Name, NormalizedName = role.NormalizedName, RoleId = role.Id };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Role role;
                if (Input.RoleId > 0)
                {
                    role = _roleManager.FindById(Input.RoleId.Value);
                    role.Name = Input.Name;
                }
                else
                    role = new Role
                    {
                        Name = Input.Name,
                        NormalizedName = Input.NormalizedName,
                        RoleLevel = 1
                    };
                var action = Input.RoleId > 0 ? "更新" : "添加";
                var result = await _roleManager.SaveAsync(role);
                if (result.Succeeded)
                {
                    EventLogger.LogUser("{0}了用户角色：{1}。", action, Input.Name);
                    return Success($"你已经成功{action}角色“{Input.Name}”。");
                }
                return Error(result.ToErrorString());
            }
            return Error();
        }
    }
}