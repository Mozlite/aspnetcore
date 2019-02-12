using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信。
    /// </summary>
    [Table("core_SMS")]
    public class Note : IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 消息。
        /// </summary>
        [Size(256)]
        public string Message { get; set; }

        /// <summary>
        /// 电话号码。
        /// </summary>
        [Size(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 尝试发送次数。
        /// </summary>
        public int TryTimes { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public NoteStatus Status { get; set; }

        /// <summary>
        /// 发送返回的消息。
        /// </summary>
        [Size(256)]
        public string Msg { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 发送时间。
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>
        /// 发送器名称。
        /// </summary>
        [Size(36)]
        public string Client { get; set; }

        private string _hashkey;
        /// <summary>
        /// 唯一键验证。
        /// </summary>
        [Size(32)]
        [NotUpdated]
        public string HashKey
        {
            get => _hashkey ?? (_hashkey = Cores.Md5($"{PhoneNumber}::{Message}"));
            set => _hashkey = value;
        }
    }
}