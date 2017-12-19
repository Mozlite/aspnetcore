using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Messages.Models
{
    /// <summary>
    /// 信息实体类。
    /// </summary>
    [Table("core_Messages")]
    public class Message
    {
        /// <summary>
        /// 信息Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 信息类型。
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// 标题，如果是短信则表示内容。
        /// </summary>
        [Size(256)]
        public string Title { get; set; }

        /// <summary>
        /// 内容。
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 电子邮件地址或者电话号码。
        /// </summary>
        [Size(256)]
        public string To { get; set; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 尝试发送次数。
        /// </summary>
        public int TryTimes { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public MessageStatus Status { get; set; }

        /// <summary>
        /// 添加日期。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 发送/失败日期，或者已读日期。
        /// </summary>
        public DateTimeOffset? ConfirmDate { get; set; }
    }
}