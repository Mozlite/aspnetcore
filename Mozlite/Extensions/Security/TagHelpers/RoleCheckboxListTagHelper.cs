using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Services;
using Mozlite.Mvc.TagHelpers.Bootstrap;

namespace Mozlite.Extensions.Security.TagHelpers
{
    [HtmlTargetElement("moz:rolecheckboxlist")]
    public class RoleCheckboxListTagHelper : RadioboxListTagHelper
    {
        private readonly IRoleManager _roleManager;

        public RoleCheckboxListTagHelper(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            foreach (var role in _roleManager.Load())
            {
                items.Add(role.DisplayName, role.RoleId.ToString());
            }
        }
    }
}