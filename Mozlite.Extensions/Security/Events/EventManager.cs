using Mozlite.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// 加载用户最新的事件消息列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="size">记录数。</param>
        /// <returns>返回事件消息列表。</returns>
        public virtual IEnumerable<EventMessage> LoadMessages(int userId, int size = 10)
        {
            return Context.AsQueryable()
                .WithNolock()
                .Select()
                .InnerJoin<EventType>((e, t) => e.EventId == t.Id)
                .Select<EventType>(x => x.Name)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .AsEnumerable(size);
        }

        /// <summary>
        /// 加载用户最新的事件消息列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="size">记录数。</param>
        /// <returns>返回事件消息列表。</returns>
        public virtual Task<IEnumerable<EventMessage>> LoadMessagesAsync(int userId, int size = 10)
        {
            return Context.AsQueryable()
                .WithNolock()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .AsEnumerableAsync(size);
        }
    }
}