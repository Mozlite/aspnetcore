using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Properties;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Tasks;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 邮件发送服务。
    /// </summary>
    public class EmailSendTaskService : TaskService
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IMessageManager _messageManager;
        private readonly ILogger<EmailSendTaskService> _logger;
        /// <summary>
        /// 初始化类<see cref="EmailSendTaskService"/>。
        /// </summary>
        /// <param name="settingsManager">配置管理接口。</param>
        /// <param name="messageManager">消息管理接口。</param>
        /// <param name="logger">日志接口。</param>
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
            var messages = await _messageManager.LoadAsync(MessageStatus.Pending);
            if (!messages.Any()) return;
            foreach (var message in messages)
            {
                try
                {
                    await SendAsync(settings, message);
                    await _messageManager.SetSuccessAsync(message.Id);
                }
                catch (Exception exception)
                {
                    await _messageManager.SetFailuredAsync(message.Id, settings.MaxTryTimes);
                    _logger.LogError(exception, "发送邮件错误");
                }
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="settings">网站配置。</param>
        /// <param name="message">消息实例。</param>
        /// <returns>返回发送任务。</returns>
        protected virtual async Task SendAsync(EmailSettings settings, Email message)
        {
            using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
            {
                client.EnableSsl = settings.UseSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(settings.SmtpUserName, settings.SmtpPassword);
                var mail = new MailMessage();
                mail.From = new MailAddress(settings.SmtpUserName);
                mail.To.Add(message.To);
                mail.Body = message.Content;
                mail.Subject = message.Title;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                await InitAsync(mail, message);
                await client.SendMailAsync(mail);
            }
        }

        /// <summary>
        /// 实例化一个电子邮件。
        /// </summary>
        /// <param name="mail">邮件实例。</param>
        /// <param name="message">消息实例。</param>
        /// <returns>返回邮件实例对象。</returns>
        protected virtual Task InitAsync(MailMessage mail, Email message) => Task.CompletedTask;
    }
}