using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件管理接口。
    /// </summary>
    public interface IEventManager : IObjectManager<EventMessage>, ISingletonService
    {
        /// <summary>
        /// 加载用户最新的事件消息列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="size">记录数。</param>
        /// <returns>返回事件消息列表。</returns>

        IEnumerable<EventMessage> LoadMessages(int userId, int size = 10);

        /// <summary>
        /// 加载用户最新的事件消息列表。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="size">记录数。</param>
        /// <returns>返回事件消息列表。</returns>

        Task<IEnumerable<EventMessage>> LoadMessagesAsync(int userId, int size = 10);
    }
}