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
        /// 获取资源，一般为内容。
        /// </summary>
        /// <param name="resourceKey">资源见。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <returns></returns>
        string GetTemplate(string resourceKey, object replacement = null);

        /// <summary>
        /// 获取资源，一般为内容。
        /// </summary>
        /// <typeparam name="TResource">资源类型。</typeparam>
        /// <param name="resourceKey">资源见。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <returns></returns>
        string GetTemplate<TResource>(string resourceKey, object replacement = null);

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
        bool Save(Email message);

        /// <summary>
        /// 添加消息接口。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> SaveAsync(Email message);

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Email.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        bool IsExisted(Email message, int expiredSeconds = 300);

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Email.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsExistedAsync(Email message, int expiredSeconds = 300);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        bool SendEmail(int userId, string emailAddress, string title, string content, Action<Email> action = null);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="emailAddress">电子邮件地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content, Action<Email> action = null);
        
        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        IEnumerable<Email> Load(MessageStatus? status = null);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        Task<IEnumerable<Email>> LoadAsync(MessageStatus? status = null);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        TQuery Load<TQuery>(TQuery query) where TQuery : EmailQuery;

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : EmailQuery;

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

        /// <summary>
        /// 通过Id查询消息。
        /// </summary>
        /// <param name="id">消息id。</param>
        /// <returns>返回消息实例。</returns>
        Email Find(int id);

        /// <summary>
        /// 通过Id查询消息。
        /// </summary>
        /// <param name="id">消息id。</param>
        /// <returns>返回消息实例。</returns>
        Task<Email> FindAsync(int id);
    }
}