using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件类型管理。
    /// </summary>
    public class EventTypeManager : CachableCategoryManager<EventType>, IEventTypeManager
    {
        /// <summary>
        /// 初始化类<see cref="EventTypeManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public EventTypeManager(IDbContext<EventType> context, IMemoryCache cache)
            : base(context, cache)
        {
        }
    }
}