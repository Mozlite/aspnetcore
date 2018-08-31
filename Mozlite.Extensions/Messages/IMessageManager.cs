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
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(Message message);

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(Message message);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        bool SendEmail(int userId, string emailAddress, string title, string content);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content);

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <returns>返回发送结果。</returns>
        bool SendSMS(int userId, string phoneNumber, string message);

        /// <summary>
        /// 发送短信。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendSMSAsync(int userId, string phoneNumber, string message);

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        bool SendMessage(int userId, string title, string content);

        /// <summary>
        /// 发送系统消息。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendMessageAsync(int userId, string title, string content);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="messageType">消息类型。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        Task<IEnumerable<Message>> LoadAsync(MessageType messageType, MessageStatus? status = null);

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
        Task<bool> SetSuccessAsync(int id);
    }
}