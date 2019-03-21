using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Security.Events;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Notifications
{
    public class EditModel : AdminModelBase
    {
        private readonly INotificationTypeManager _typeManager;

        public EditModel(INotificationTypeManager typeManager)
        {
            _typeManager = typeManager;
        }

        [BindProperty]
        public NotificationType Input { get; set; }

        public void OnGet(int id)
        {
            Input = _typeManager.Find(id) ?? new NotificationType();
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Input.Name))
            {
                ModelState.AddModelError("Input.Name", "名称不能为空！");
                return Error();
            }

            var result = await _typeManager.SaveAsync(Input);
            if (result)
            {
                await EventLogger.LogCoreResultAsync(result, "通知类型：{0}。", Input.Name);
            }

            return Json(result, Input.Name);
        }
    }
}