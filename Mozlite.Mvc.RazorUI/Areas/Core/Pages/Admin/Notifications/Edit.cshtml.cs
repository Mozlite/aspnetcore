using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages.Notifications;
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

            var action = Input.Id > 0 ? "更新" : "添加";
            var result = await _typeManager.SaveAsync(Input);
            if (result)
            {
                Log("{1}了通知类型：{0}。", action, Input.Name);
            }

            return Json(result, Input.Name);
        }
    }
}