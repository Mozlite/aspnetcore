using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Tasks;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Tasks
{
    [PermissionAuthorize(Permissions.TaskInterval)]
    public class IntervalModel : ModelBase
    {
        private readonly ITaskManager _taskManager;
        public IntervalModel(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public TaskDescriptor Task { get; private set; }

        public IHtmlContent DefaultInterval { get; private set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet(int id)
        {
            Task = _taskManager.GeTask(id);
            if (Task == null)
                return NotFound();
            TaskInterval interval = Task.Interval;
            DefaultInterval = interval.ToHtmlString();
            Input = new InputModel { Id = Task.Id, Interval = Task.TaskArgument.Interval ?? Task.Interval };
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            TaskInterval interval = null;
            if (!string.IsNullOrEmpty(Input.Interval))
            {
                try
                {
                    interval = Input.Interval;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Input.Interval", "时间间隔格式错误，请输入正确的格式！");
                    return Error();
                }
            }

            Task = _taskManager.GeTask(Input.Id);
            if (await _taskManager.SaveArgumentIntervalAsync(Input.Id, interval?.ToString()))
            {
                interval = Task.Interval;
                await EventLogger.LogCoreAsync("将后台服务 {2} 的时间间隔由 {0} 修改为 {1}。", Task.ToHtmlInterval().ToString(), interval.ToHtmlString().ToString(),
                    Task.Name);
                return Success("你已经成功更改了时间间隔！");
            }
            return Error();
        }

        public class InputModel
        {
            public int Id { get; set; }

            public string Interval { get; set; }
        }
    }
}