using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers.Bootstrap;

namespace Demo.Extensions.Security.TagHelpers
{
    [HtmlTargetElement("demo:role-checkboxlist")]
    public class RoleCheckboxListTagHelper : CheckboxListTagHelper
    {
        private readonly IRoleManager _roleManager;
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