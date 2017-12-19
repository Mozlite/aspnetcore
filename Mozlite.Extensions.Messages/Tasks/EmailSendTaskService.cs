using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Messages.Models;
using Mozlite.Extensions.Messages.Properties;
using Mozlite.Extensions.Messages.Services;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Tasks;

namespace Mozlite.Extensions.Messages.Tasks
{
    /// <summary>
    /// 邮件发送服务。
    /// </summary>
    public class EmailSendTaskService : TaskService
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IMessageManager _messageManager;
        private readonly ILogger<EmailSendTaskService> _logger;

        public EmailSendTaskService(ISettingsManager settingsManager, IMessageManager messageManager, ILogger<EmailSendTaskService> logger)
        {
            _settingsManager = settingsManager;
            _messageManager = messageManager;
            _logger = logger;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => Resources.EmailTaskService;

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => Resources.EmailTaskService_Description;

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public override TaskInterval Interval => 60;

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override async Task ExecuteAsync(Argument argument)
        {
            var settings = await _settingsManager.GetSettingsAsync<EmailSettings>();
            if (!settings.Enabled) return;
            var messages = await _messageManager.LoadAsync(MessageType.Email, MessageStatus.Pending);
            if (!messages.Any()) return;
            foreach (var message in messages)
            {
                try
                {
                    await SendAsync(settings, message);
                }
                catch (Exception exception)
                {
                    await _messageManager.SetFailuredAsync(message.Id, settings.MaxTryTimes);
                    _logger.LogError(exception, "发送邮件错误");
                }
                await Task.Delay(100);
            }
        }

        private async Task SendAsync(EmailSettings settings, Message message)
        {
            using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
            {
                client.EnableSsl = settings.UseSsl;
                client.Credentials = new NetworkCredential(settings.SmtpUserName, settings.SmtpPassword);
                await client.SendMailAsync(CreateMessage(settings.SmtpUserName, message));
            }
            await _messageManager.SetSuccessAsync(message.Id);
        }

        private MailMessage CreateMessage(string from, Message message)
        {
            var mail = new MailMessage(from, message.To, message.Title, message.Content);
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            return mail;
        }
    }
}