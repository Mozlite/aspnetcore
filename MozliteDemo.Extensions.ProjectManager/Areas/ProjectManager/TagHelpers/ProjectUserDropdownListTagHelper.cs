using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite;
using Mozlite.Mvc.TagHelpers;
using MozliteDemo.Extensions.ProjectManager.Projects;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 用户下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:project-user-dropdownlist")]
    public class ProjectUserDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IProjectUserManager _userManager;

        public ProjectUserDropdownListTagHelper(IProjectUserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            var users = _userManager.Fetch();
            foreach (var user in users)
            {
                yield return new SelectListItem(user.UserName, user.Id.ToString());
            }
        }
    }
}