using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Storages;
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
    }
}