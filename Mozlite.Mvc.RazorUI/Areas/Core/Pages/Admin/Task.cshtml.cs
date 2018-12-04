using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Tasks;

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
    }
}