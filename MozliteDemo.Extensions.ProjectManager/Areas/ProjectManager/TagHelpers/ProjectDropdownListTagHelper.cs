using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers;
using MozliteDemo.Extensions.ProjectManager.Projects;
using System.Collections.Generic;
using System.Linq;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.TagHelpers
{
    /// <summary>
    /// 项目下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:project-dropdownlist")]
    public class ProjectDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IProjectManager _projectManager;

        public ProjectDropdownListTagHelper(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        /// <summary>
        /// 项目负责人Id。
        /// </summary>
        [HtmlAttributeName("userid")]
        public int UserId { get; set; }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            var projects = _projectManager.Fetch(x => x.Enabled);
            if (UserId > 0)
                projects = projects.Where(x => x.UserId == UserId);
            foreach (var project in projects)
            {
                yield return new SelectListItem(project.Name, project.Id.ToString());
            }
        }
    }
}