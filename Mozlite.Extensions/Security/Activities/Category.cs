using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类。
    /// </summary>
    [Table("core_Users_Activities_Categories")]
    public class Category : CategoryBase
    {
        /// <summary>
        /// 默认事件ID。
        /// </summary>
        public static readonly EventId EventId = Create(0);

        /// <summary>
        /// 隐式转换事件ID。
        /// </summary>
        /// <param name="category">当前分类实例。</param>
        public static implicit operator EventId(Category category)
        {
            return new EventId(category.Id, EventId.Name);
        }

        internal static EventId Create(int eventId)
        {
            return new EventId(eventId, "{:user<->activity:}");
        }
    }
}