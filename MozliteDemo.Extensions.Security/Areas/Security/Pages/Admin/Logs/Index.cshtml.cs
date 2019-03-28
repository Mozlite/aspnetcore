using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Logs
{
    /// <summary>
    /// 日志。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.Logs)]
    public class IndexModel : ModelBase
    {
        private readonly IEventManager _eventManager;
        private readonly IEventTypeManager _eventTypeManager;

        public IndexModel(IEventManager eventManager, IEventTypeManager eventTypeManager)
        {
            _eventManager = eventManager;
            _eventTypeManager = eventTypeManager;
        }

        /// <summary>
        /// 获取事件类型。
        /// </summary>
        /// <param name="id">事件类型Id。</param>
        /// <returns>返回事件类型名称。</returns>
        public string GetEventType(int id) => _eventTypeManager.Find(id)?.Name;

        [BindProperty(SupportsGet = true)]
        public EventQuery Model { get; set; }

        public void OnGet()
        {
            Model.RoleLevel = Role.RoleLevel;
            Model = _eventManager.Load(Model);
        }
    }
}