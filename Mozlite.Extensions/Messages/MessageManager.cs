using Mozlite.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 消息管理实现类。
    /// </summary>
    public class MessageManager : IMessageManager
    {
        /// <summary>
        /// 数据库操作接口实例。
        /// </summary>
        protected IDbContext<Message> Context { get; }

        /// <summary>
        /// 初始化类<see cref="MessageManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        public MessageManager(IDbContext<Message> context)
        {
            Context = context;
        }

        /// <summary>
        /// 更新列。
        /// </summary>
        /// <param name="id">当前Id。</param>
        /// <param name="fields">更新匿名对象。</param>
        /// <returns>返回更新结果。</returns>
        public virtual bool Update(int id, object fields) => Context.Update(id, fields);

        /// <summary>
        /// 更新列。
        /// </summary>
        /// <param name="id">当前Id。</param>
        /// <param name="fields">更新匿名对象。</param>
        /// <returns>返回更新结果。</returns>
        public virtual Task<bool> UpdateAsync(int id, object fields) => Context.UpdateAsync(id, fields);

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Save(Message message)
        {
            if (message.Id > 0)
                return Context.Update(message);
            return Context.Create(message);
        }

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> SaveAsync(Message message)
        {
            if (message.Id > 0)
                return Context.UpdateAsync(message);
            return Context.CreateAsync(message);
        }

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Message.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsExisted(Message message, int expiredSeconds = 300)
        {
            if (message.Id > 0)
                return true;
            var msg = Context.Find(x => x.HashKey == message.HashKey);
            if (msg == null)
                return false;
            return msg.CreatedDate.AddSeconds(expiredSeconds) > DateTimeOffset.Now;
        }

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Message.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> IsExistedAsync(Message message, int expiredSeconds = 300)
        {
            if (message.Id > 0)
                return true;
            var msg = await Context.FindAsync(x => x.HashKey == message.HashKey);
            if (msg == null)
                return false;
            return msg.CreatedDate.AddSeconds(expiredSeconds) > DateTimeOffset.Now;
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual bool SendEmail(int userId, string emailAddress, string title, string content, Action<Message> action = null)
        {
            var message = new Message();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
            message.MessageType = MessageType.Email;
            message.Content = content;
            action?.Invoke(message);
            return Save(message);
        }

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content, Action<Message> action = null)
        {
            var message = new Message();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
            message.MessageType = MessageType.Email;
            message.Content = content;
            action?.Invoke(message);
            return SaveAsync(message);
        }

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual bool SendSMS(int userId, string phoneNumber, string message, Action<Message> action = null)
        {
            var msg = new Message();
            msg.UserId = userId;
            msg.To = phoneNumber;
            msg.Title = message;
            msg.MessageType = MessageType.SMS;
            action?.Invoke(msg);
            return Save(msg);
        }

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual Task<bool> SendSMSAsync(int userId, string phoneNumber, string message, Action<Message> action = null)
        {
            var msg = new Message();
            msg.UserId = userId;
            msg.To = phoneNumber;
            msg.Title = message;
            msg.MessageType = MessageType.SMS;
            action?.Invoke(msg);
            return SaveAsync(msg);
        }

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual bool SendMessage(int userId, string title, string content, Action<Message> action = null)
        {
            var message = new Message();
            message.UserId = userId;
            message.Title = title;
            message.MessageType = MessageType.Message;
            message.Content = content;
            action?.Invoke(message);
            return Save(message);
        }

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        public virtual Task<bool> SendMessageAsync(int userId, string title, string content, Action<Message> action = null)
        {
            var message = new Message();
            message.UserId = userId;
            message.Title = title;
            message.MessageType = MessageType.Message;
            message.Content = content;
            action?.Invoke(message);
            return SaveAsync(message);
        }

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        public virtual IEnumerable<Message> Load(MessageType messageType, MessageStatus? status = null)
        {
            var query = Context.AsQueryable();
            query.Where(x => x.MessageType == messageType);
            if (status != null)
                query.Where(x => x.Status == status);
            query.OrderBy(x => x.Id);
            return query.AsEnumerable(100);
        }

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        public virtual Task<IEnumerable<Message>> LoadAsync(MessageType messageType, MessageStatus? status = null)
        {
            var query = Context.AsQueryable();
            query.Where(x => x.MessageType == messageType);
            if (status != null)
                query.Where(x => x.Status == status);
            query.OrderBy(x => x.Id);
            return query.AsEnumerableAsync(100);
        }

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : MessageQueryBase => Context.Load(query);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : MessageQueryBase => Context.LoadAsync(query);

        /// <summary>
        /// 设置失败状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <param name="maxTryTimes">最大失败次数。</param>
        /// <returns>返回设置结果。</returns>
        public virtual bool SetFailured(int id, int maxTryTimes)
        {
            return Context.BeginTransaction(db =>
            {
                db.Update(x => x.Id == id, x => new { TryTimes = x.TryTimes + 1 });
                db.Update(x => x.Id == id && x.TryTimes > maxTryTimes,
                    new { Status = MessageStatus.Failured, ConfirmDate = DateTimeOffset.Now });
                return true;
            });
        }

        /// <summary>
        /// 设置失败状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <param name="maxTryTimes">最大失败次数。</param>
        /// <returns>返回设置结果。</returns>
        public virtual Task<bool> SetFailuredAsync(int id, int maxTryTimes)
        {
            return Context.BeginTransactionAsync(async db =>
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
        public virtual bool SetSuccess(int id) => Update(id, new { Status = MessageStatus.Completed, ConfirmDate = DateTimeOffset.Now });

        /// <summary>
        /// 设置成功状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <returns>返回设置结果。</returns>
        public virtual Task<bool> SetSuccessAsync(int id) => UpdateAsync(id, new { Status = MessageStatus.Completed, ConfirmDate = DateTimeOffset.Now });
    }
}