using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类。
    /// </summary>
    [Table("core_Users_Activities_Categories")]
    public abstract class CategoryBase : Categories.CategoryBase
    {
        /// <summary>
        /// 默认事件ID。
        /// </summary>
        public static readonly EventId EventId = Create(0);

        /// <summary>
        /// 实例化一个事件实例。
        /// </summary>
        /// <param name="categoryId">当前分类ID。</param>
        /// <returns>返回事件实例。</returns>
        public static EventId Create(int categoryId)
        {
            return new EventId(categoryId, "{:user<->activity:}");
        }
    }
}