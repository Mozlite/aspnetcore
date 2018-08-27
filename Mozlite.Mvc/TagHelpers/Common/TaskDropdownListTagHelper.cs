using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 后台服务下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:task-dropdownlist")]
    public class TaskDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly ITaskManager _taskManager;

        /// <summary>
        /// 初始化类<see cref="TaskDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="taskManager">服务管理接口。</param>
        public TaskDropdownListTagHelper(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        /// <summary>
        /// 扩展类型。
        /// </summary>
        [HtmlAttributeName("extension")]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override async Task<IEnumerable<SelectListItem>> InitAsync()
        {
            var tasks = await _taskManager.LoadTasksAsync();
            if (ExtensionName != null)
                tasks = tasks.Where(x => x.ExtensionName.Equals(ExtensionName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            return tasks.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }
    }
}