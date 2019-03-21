using Mozlite.Extensions.Categories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件类型。
    /// </summary>
    [Table("core_Users_Events_Types")]
    public class EventType : CategoryBase
    {
        /// <summary>
        /// 图标地址。
        /// </summary>
        [Size(256)]
        public string IconUrl { get; set; }

        /// <summary>
        /// 背景颜色。
        /// </summary>
        [Size(20)]
        public string BgColor { get; set; }

        /// <summary>
        /// 字体颜色。
        /// </summary>
        [Size(20)]
        public string Color { get; set; }
    }
}