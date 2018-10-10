using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 消息管理接口。
    /// </summary>
    public interface IMessageManager : ISingletonService
    {
        /// <summary>
        /// 更新列。
        /// </summary>
        /// <param name="id">当前Id。</param>
        /// <param name="fields">更新匿名对象。</param>
        /// <returns>返回更新结果。</returns>
        bool Update(int id, object fields);

        /// <summary>
        /// 更新列。
        /// </summary>
        /// <param name="id">当前Id。</param>
        /// <param name="fields">更新匿名对象。</param>
        /// <returns>返回更新结果。</returns>
        Task<bool> UpdateAsync(int id, object fields);

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        bool Save(Message message);

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> SaveAsync(Message message);

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Message.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        bool IsExisted(Message message, int expiredSeconds = 300);

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Message.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsExistedAsync(Message message, int expiredSeconds = 300);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        bool SendEmail(int userId, string emailAddress, string title, string content, Action<Message> action = null);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content, Action<Message> action = null);

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        // ReSharper disable once InconsistentNaming
        bool SendSMS(int userId, string phoneNumber, string message, Action<Message> action = null);

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        // ReSharper disable once InconsistentNaming
        Task<bool> SendSMSAsync(int userId, string phoneNumber, string message, Action<Message> action = null);

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        bool SendMessage(int userId, string title, string content, Action<Message> action = null);

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendMessageAsync(int userId, string title, string content, Action<Message> action = null);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        IEnumerable<Message> Load(MessageType messageType, MessageStatus? status = null);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        Task<IEnumerable<Message>> LoadAsync(MessageType messageType, MessageStatus? status = null);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        TQuery Load<TQuery>(TQuery query) where TQuery : MessageQueryBase;

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : MessageQueryBase;

        /// <summary>
        /// 设置失败状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <param name="maxTryTimes">最大失败次数。</param>
        /// <returns>返回设置结果。</returns>
        bool SetFailured(int id, int maxTryTimes);

        /// <summary>
        /// 设置失败状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <param name="maxTryTimes">最大失败次数。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetFailuredAsync(int id, int maxTryTimes);

        /// <summary>
        /// 设置成功状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <returns>返回设置结果。</returns>
        bool SetSuccess(int id);

        /// <summary>
        /// 设置成功状态。
        /// </summary>
        /// <param name="id">当前消息Id。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetSuccessAsync(int id);
    }
}