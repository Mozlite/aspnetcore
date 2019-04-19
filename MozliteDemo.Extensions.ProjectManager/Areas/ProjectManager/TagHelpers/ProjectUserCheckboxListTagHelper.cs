using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite;
using Mozlite.Mvc.TagHelpers.Bootstrap;
using MozliteDemo.Extensions.ProjectManager.Projects;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 项目用户复选框。
    /// </summary>
    [HtmlTargetElement("moz:project-user-checkboxlist")]
    public class ProjectUserCheckboxListTagHelper : CheckboxListTagHelper
    {
        private readonly IProjectUserManager _userManager;

        public ProjectUserCheckboxListTagHelper(IProjectUserManager userManager)
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