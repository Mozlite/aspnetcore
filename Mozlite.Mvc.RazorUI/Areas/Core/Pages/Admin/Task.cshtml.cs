using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin
{
    [PermissionAuthorize(Permissions.Task)]
    public class TaskModel : AdminModelBase
    {
        private readonly ITaskManager _taskManager;

        public TaskModel(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public IEnumerable<TaskDescriptor> Tasks { get; private set; }

        public async Task OnGetAsync()
        {
            Tasks = await _taskManager.LoadTasksAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var tasks = await _taskManager.LoadTasksAsync();
            return Success(
                tasks.Select(x => new
                {
                    x.Id,
                    LastExecuted = x.LastExecuted?.ToString("yyyy-MM-dd HH:mm:ss"),
                    NextExecuting = x.NextExecuting < DateTime.Now ? null : x.NextExecuting.ToString("yyyy-MM-dd HH:mm:ss")
                }));
        }
    }
}