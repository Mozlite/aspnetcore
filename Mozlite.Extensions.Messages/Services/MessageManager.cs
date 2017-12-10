using System.Threading.Tasks;
using Mozlite.Data;
using Mozlite.Extensions.Messages.Models;

namespace Mozlite.Extensions.Messages.Services
{
    /// <summary>
    /// 消息管理实现类。
    /// </summary>
    public class MessageManager : IMessageManager
    {
        private readonly IRepository<Message> _repository;

        public MessageManager(IRepository<Message> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public bool Create(Message message)
        {
            return _repository.Create(message);
        }

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public Task<bool> CreateAsync(Message message)
        {
            return _repository.CreateAsync(message);
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        public bool SendEmail(int userId, string emailAddress, string title, string content)
        {
            var message = new Message();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
            message.MessageType = MessageType.Email;
            message.Content = content;
            return Create(message);
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        public Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content)
        {
            var message = new Message();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
            message.MessageType = MessageType.Email;
            message.Content = content;
            return CreateAsync(message);
        }

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <returns>返回发送结果。</returns>
        public bool SendSMS(int userId, string phoneNumber, string message)
        {
            var msg = new Message();
            msg.UserId = userId;
            msg.To = phoneNumber;
            msg.Title = message;
            msg.MessageType = MessageType.SMS;
            return Create(msg);
        }

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <returns>返回发送结果。</returns>
        public Task<bool> SendSMSAsync(int userId, string phoneNumber, string message)
        {
            var msg = new Message();
            msg.UserId = userId;
            msg.To = phoneNumber;
            msg.Title = message;
            msg.MessageType = MessageType.SMS;
            return CreateAsync(msg);
        }

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        public bool SendMessage(int userId, string title, string content)
        {
            var message = new Message();
            message.UserId = userId;
            message.Title = title;
            message.MessageType = MessageType.Message;
            message.Content = content;
            return Create(message);
        }

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        public Task<bool> SendMessageAsync(int userId, string title, string content)
        {
            var message = new Message();
            message.UserId = userId;
            message.Title = title;
            message.MessageType = MessageType.Message;
            message.Content = content;
            return CreateAsync(message);
        }
    }
}