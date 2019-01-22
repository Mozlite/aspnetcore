using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 系统通知。
    /// </summary>
    [Table("core_Notifications")]
    public class Notification : IIdObject
    {
        /// <summary>
        /// 自增长Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 类型Id。
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 通知内容。
        /// </summary>
        [Size(256)]
        public string Message { get; set; }

        /// <summary>
        /// 接收通知用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 发生通知时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 通知状态。
        /// </summary>
        public NotificationStatus Status { get; set; } = NotificationStatus.New;

        /// <summary>
        /// 类型名称。
        /// </summary>
        [NotMapped]
        public string Name { get; set; }

        /// <summary>
        /// 图标地址。
        /// </summary>
        [NotMapped]
        public string IconUrl { get; set; }
    }
}