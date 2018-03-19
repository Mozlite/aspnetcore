using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类辅助类。
    /// </summary>
    public class CategoryHelper
    {
        /// <summary>
        /// 事件名称。
        /// </summary>
        public const string EventName = "{:user<->activity:}";

        /// <summary>
        /// 用户活动日志默认事件实例。
        /// </summary>
        public static readonly EventId EventId = Create(0);

        /// <summary>
        /// 实例化一个事件实例。
        /// </summary>
        /// <param name="categoryId">当前分类ID。</param>
        /// <returns>返回事件实例。</returns>
        public static EventId Create(int categoryId)
        {
            return new EventId(categoryId, EventName);
        }
    }
}