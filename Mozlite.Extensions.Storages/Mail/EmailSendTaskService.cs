using Microsoft.Extensions.Logging;
using MimeKit;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Settings;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Mail
{
    /// <summary>
    /// 邮件发送服务。
    /// </summary>
    public abstract class EmailSendTaskService : Messages.EmailSendTaskService
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
        protected EmailSendTaskService(ISettingsManager settingsManager, IMessageManager messageManager, ILogger<EmailSendTaskService> logger, IMediaDirectory mediaDirectory)
            : base(settingsManager, messageManager, logger)
        {
            MediaDirectory = mediaDirectory;
        }

        /// <summary>
        /// 实例化一个电子邮件。
        /// </summary>
        /// <param name="mail">邮件实例。</param>
        /// <param name="message">消息实例。</param>
        /// <returns>返回邮件实体对象。</returns>
        protected override async Task<Multipart> InitAsync(MimeMessage mail, Email message)
        {
            var attachments = message.GetAttachments().ToList();
            if (attachments?.Count > 0)
            {
                var multipart = new Multipart("mixed");
                foreach (var attachmentId in attachments)
                {
                    var file = await MediaDirectory.FindAsync(attachmentId);
                    if (file == null)
                        continue;
                    var attachment = new MimePart(file.ContentType);
                    attachment.Content = new MimeContent(File.OpenRead(file.PhysicalPath));
                    attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                    attachment.ContentTransferEncoding = ContentEncoding.Default;
                    attachment.FileName = file.FileName;
                    multipart.Add(attachment);
                }

                return multipart;
            }
            return null;
        }
    }
}