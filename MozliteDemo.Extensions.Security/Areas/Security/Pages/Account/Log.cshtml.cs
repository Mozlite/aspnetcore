using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Account
{
    /// <summary>
    /// 日志模型。
    /// </summary>
    public class LogModel : ModelBase
    {
        private readonly IEventManager _eventManager;
        private readonly IEventTypeManager _eventTypeManager;

        public LogModel(IEventManager eventManager, IEventTypeManager eventTypeManager)
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
            Model.UserId = UserId;
            Model = _eventManager.Load(Model);
        }
    }
}