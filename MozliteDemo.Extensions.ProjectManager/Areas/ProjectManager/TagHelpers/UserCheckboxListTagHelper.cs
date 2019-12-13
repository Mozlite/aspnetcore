using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite;
using Mozlite.Mvc.TagHelpers.Bootstrap;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 用户复选框。
    /// </summary>
    [HtmlTargetElement("moz:user-checkboxlist")]
    public class UserCheckboxListTagHelper : CheckboxListTagHelper
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public UserCheckboxListTagHelper(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            var roles = _roleManager.Load().Where(x => !x.IsSystem).Select(x => x.Id).ToList();
            var users = _userManager.LoadUsers(x => x.RoleId.Included(roles));
            foreach (var user in users)
            {
                items[user.UserName] = user.Id.ToString();
            }
        }
    }
}