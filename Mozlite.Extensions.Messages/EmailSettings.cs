namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 电子邮件配置。
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// 启用SMTP服务器。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// SMTP地址。
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// SMTP地址。
        /// </summary>
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
        public string SmtpPassword { get; set; }
    }
}