using Microsoft.Extensions.Logging;
using MimeKit;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Mail;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailSendTaskService = Mozlite.Extensions.Storages.Mail.EmailSendTaskService;

namespace Mozlite.Mvc.RazorUI.Extensions.Messages
{
    /// <summary>
    /// 邮件发送服务。
    /// </summary>
    public class EmailTaskService : EmailSendTaskService
    {
        /// <summary>
        /// 初始化类<see cref="EmailTaskService"/>。
        /// </summary>
        /// <param name="settingsManager">配置管理接口。</param>
        /// <param name="messageManager">消息管理接口。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="mediaDirectory">媒体文件操作接口。</param>
        public EmailTaskService(ISettingsManager settingsManager, IMessageManager messageManager, ILogger<EmailSendTaskService> logger, IMediaDirectory mediaDirectory)
            : base(settingsManager, messageManager, logger, mediaDirectory)
        {
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="settings">网站配置。</param>
        /// <param name="message">消息实例。</param>
        /// <returns>返回发送任务。</returns>
        protected override async Task SendAsync(EmailSettings settings, Email message)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, settings.UseSsl);
                await client.AuthenticateAsync(settings.SmtpUserName, settings.SmtpPassword);

                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(settings.SmtpUserName));
                mail.To.Add(new MailboxAddress(message.To));
                mail.Subject = message.Title;
                var html = new TextPart("Html") { Text = message.Content };

                var attachments = message.GetAttachments().ToList();
                if (attachments?.Count > 0)
                {
                    var multipart = new Multipart("mixed");
                    multipart.Add(html);
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

                    mail.Body = multipart;
                }
                else
                {
                    mail.Body = html;
                }

                await client.SendAsync(mail);
            }
        }
    }
}