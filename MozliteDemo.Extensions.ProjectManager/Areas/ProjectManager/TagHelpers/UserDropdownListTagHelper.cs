using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite;
using Mozlite.Mvc.TagHelpers;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 用户下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:user-dropdownlist")]
    public class UserDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public UserDropdownListTagHelper(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            var roles = _roleManager.Load().Where(x => !x.IsSystem).Select(x => x.RoleId).ToList();
            var users = _userManager.LoadUsers(x => x.RoleId.Included(roles));
            foreach (var user in users)
            {
                yield return new SelectListItem(user.UserName, user.UserId.ToString());
            }
        }
    }
}