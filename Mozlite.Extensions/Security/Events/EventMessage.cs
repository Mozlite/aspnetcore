using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件消息。
    /// </summary>
    [Table("core_Users_Events")]
    public class EventMessage : ExtendBase, IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 事件类型Id。
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// 当前用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 活动时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// IP地址。
        /// </summary>
        [Size(32)]
        // ReSharper disable once InconsistentNaming
        public string IPAdress { get; set; }

        /// <summary>
        /// 操作日志。
        /// </summary>
        [NotMapped]
        public string Message
        {
            get => this[nameof(Message)]; set => this[nameof(Message)] = value;
        }
    }
}