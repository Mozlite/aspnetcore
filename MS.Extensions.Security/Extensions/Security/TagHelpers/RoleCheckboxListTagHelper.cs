using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers.Bootstrap;

namespace MS.Extensions.Security.TagHelpers
{
    /// <summary>
    /// 用户组列表。
    /// </summary>
    [HtmlTargetElement("moz:role-checkboxlist")]
    public class RoleCheckboxListTagHelper : CheckboxListTagHelper
    {
        private readonly IRoleManager _roleManager;
        /// <summary>
        /// 初始化类<see cref="RoleCheckboxListTagHelper"/>。
        /// </summary>
        /// <param name="roleManager">用户组管理接口。</param>
        public RoleCheckboxListTagHelper(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// 最高级角色。
        /// </summary>
        [HtmlAttributeName("max")]
        public int MaxRoleLevel { get; set; }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            foreach (var role in _roleManager.Load())
            {
                if (MaxRoleLevel > 0 && role.RoleLevel >= MaxRoleLevel || role.IsSystem)
                    continue;
                items.Add(role.Name, role.RoleId.ToString());
            }
        }
    }
}