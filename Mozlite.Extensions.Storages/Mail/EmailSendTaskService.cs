using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Settings;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Mail
{
    /// <summary>
    /// 邮件发送服务。
    /// </summary>
    [Suppress(typeof(Messages.EmailSendTaskService))]
    public class EmailSendTaskService : Messages.EmailSendTaskService
    {
        /// <summary>
        /// 媒体文件夹操作接口。
        /// </summary>
        protected IMediaDirectory MediaDirectory { get; }

        /// <summary>
        /// 初始化类<see cref="EmailSendTaskService"/>。
        /// </summary>
        /// <param name="settingsManager">配置管理接口。</param>
        /// <param name="messageManager">消息管理接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="mediaDirectory">媒体文件操作接口。</param>
        public EmailSendTaskService(ISettingsManager settingsManager, IMessageManager messageManager, ILogger<EmailSendTaskService> logger, IMediaDirectory mediaDirectory)
            : base(settingsManager, messageManager, logger)
        {
            MediaDirectory = mediaDirectory;
        }

        /// <summary>
        /// 实例化一个电子邮件。
        /// </summary>
        /// <param name="mail">邮件实例。</param>
        /// <param name="message">消息实例。</param>
        /// <returns>返回邮件实例对象。</returns>
        protected override async Task InitAsync(MailMessage mail, Email message)
        {
            var attachments = message.GetAttachments();
            if (!attachments.Any())
                return;
            foreach (var attachmentId in attachments)
            {
                var file = await MediaDirectory.FindAsync(attachmentId);
                if (file == null)
                    continue;
                var attachment = new Attachment(file.PhysicalPath, file.ContentType);
                attachment.Name = file.FileName;
                mail.Attachments.Add(attachment);
            }
        }
    }
}