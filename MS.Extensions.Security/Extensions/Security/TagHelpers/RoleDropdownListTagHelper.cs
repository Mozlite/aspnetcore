using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers;

namespace MS.Extensions.Security.TagHelpers
{
    /// <summary>
    /// 角色下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:role-dropdownlist")]
    public class RoleDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IRoleManager _roleManager;
        /// <summary>
        /// 初始化类<see cref="RoleDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="roleManager">角色管理接口。</param>
        public RoleDropdownListTagHelper(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// 最高级角色。
        /// </summary>
        [HtmlAttributeName("max")]
        public int MaxRoleLevel { get; set; }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            foreach (var role in _roleManager.Load())
            {
                if (MaxRoleLevel > 0 && role.RoleLevel >= MaxRoleLevel || role.IsSystem)
                    continue;
                yield return new SelectListItem(role.Name, role.RoleId.ToString());
            }
        }
    }
}