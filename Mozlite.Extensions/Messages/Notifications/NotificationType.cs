using System.ComponentModel.DataAnnotations.Schema;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 系统通知类型。
    /// </summary>
    [Table("core_Notifications_Types")]
    public class NotificationType : CategoryBase
    {
        /// <summary>
        /// 图标。
        /// </summary>
        [Size(256)]
        public string IconUrl { get; set; }
    }
}