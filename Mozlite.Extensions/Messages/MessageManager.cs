using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 消息管理实现类。
    /// </summary>
    public class MessageManager : IMessageManager
    {
        private readonly IDbContext<Message> _db;
        /// <summary>
        /// 初始化类<see cref="MessageManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        public MessageManager(IDbContext<Message> db)
        {
            _db = db;
        }

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public bool Create(Message message)
        {
            return _db.Create(message);
        }

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public Task<bool> CreateAsync(Message message)
        {
            return _db.CreateAsync(message);
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

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        public Task<IEnumerable<Message>> LoadAsync(MessageType messageType, MessageStatus? status = null)
        {
            var query = _db.AsQueryable();
            query.Where(x => x.MessageType == messageType);
            if (status != null)
                query.Where(x => x.Status == status);
            query.OrderBy(x => x.Id);
            return query.AsEnumerableAsync(100);
        }

        /// <summary>
        /// 设置失败状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <param name="maxTryTimes">最大失败次数。</param>
        /// <returns>返回设置结果。</returns>
        public Task<bool> SetFailuredAsync(int id, int maxTryTimes)
        {
            return _db.BeginTransactionAsync(async db =>
            {
                await db.UpdateAsync(x => x.Id == id, x => new { TryTimes = x.TryTimes + 1 });
                await db.UpdateAsync(x => x.Id == id && x.TryTimes > maxTryTimes,
                    new { Status = MessageStatus.Failured, ConfirmDate = DateTimeOffset.Now });
                return true;
            });
        }

        /// <summary>
        /// 设置成功状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <returns>返回设置结果。</returns>
        public Task<bool> SetSuccessAsync(int id)
        {
            return _db.UpdateAsync(x => x.Id == id, new { Status = MessageStatus.Completed, ConfirmDate = DateTimeOffset.Now });
        }
    }
}