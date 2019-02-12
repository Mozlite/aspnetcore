using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 电子邮件配置。
    /// </summary>
    [Table("core_Emails_Settings")]
    public class EmailSettings : IIdObject
    {
        /// <summary>
        /// 启用SMTP服务器。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// SMTP地址。
        /// </summary>
        [Size(64)]
        public string SmtpServer { get; set; }

        /// <summary>
        /// SMTP地址。
        /// </summary>
        [Size(64)]
        public string SmtpUserName { get; set; }

        /// <summary>
        /// 端口。
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// 使用SSL。
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        [Size(64)]
        public string SmtpPassword { get; set; }

        /// <summary>
        /// 最大发送次数。
        /// </summary>
        public int MaxTryTimes { get; set; } = 5;

        /// <summary>
        /// 发送个数。
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }
    }
}