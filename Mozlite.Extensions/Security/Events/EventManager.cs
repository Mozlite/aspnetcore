using Mozlite.Data;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件管理。
    /// </summary>
    public class EventManager : ObjectManager<EventMessage>, IEventManager
    {
        /// <summary>
        /// 初始化类<see cref="EventManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        public EventManager(IDbContext<EventMessage> context)
            : base(context)
        {
        }
    }
}