using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using Mozlite.Mvc.TagHelpers.Bootstrap;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 用户单选列表。
    /// </summary>
    [HtmlTargetElement("moz:user-radioboxlist")]
    public class UserRadioboxListTagHelper : RadioboxListTagHelper
    {
        private readonly IProjectUserManager _userManager;

        public UserRadioboxListTagHelper(IProjectUserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            var users = _userManager.Fetch();
            foreach (var user in users)
            {
                items[user.UserName] = user.Id.ToString();
            }
        }
    }
}
