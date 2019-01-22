using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Security.Permissions;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Controllers
{
    /// <summary>
    /// 通知控制器。
    /// </summary>
    [PermissionAuthorize(Permissions.Notifications)]
    public class NotifierController : ControllerBase
    {
        private readonly INotificationManager _notificationManager;

        public NotifierController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// 获取通知。
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationManager.LoadAsync();
            return Success(notifications);
        }

        /// <summary>
        /// 确认。
        /// </summary>
        /// <param name="id">通知Id。</param>
        [HttpPost]
        public async Task<IActionResult> Confirmed(int id)
        {
            await _notificationManager.SetStatusAsync(id, NotificationStatus.Confirmed);
            return Success();
        }

        /// <summary>
        /// 清空当前用户的通知。
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await _notificationManager.ClearAsync();
            return Success();
        }
    }
}