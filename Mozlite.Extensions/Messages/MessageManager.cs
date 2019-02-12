using Mozlite.Data;
using Mozlite.Mvc;
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
        private readonly ILocalizer _localizer;

        /// <summary>
        /// 数据库操作接口实例。
        /// </summary>
        protected IDbContext<Email> Context { get; }

        /// <summary>
        /// 初始化类<see cref="MessageManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        /// <param name="localizer">本地化接口。</param>
        public MessageManager(IDbContext<Email> context, ILocalizer localizer)
        {
            _localizer = localizer;
            Context = context;
        }

        /// <summary>
        /// 获取资源，一般为内容。
        /// </summary>
        /// <param name="resourceKey">资源见。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <returns></returns>
        public virtual string GetTemplate(string resourceKey, object replacement = null)
        {
            resourceKey = _localizer.GetString(resourceKey);
            if (replacement != null)
                resourceKey = ReplaceTemplate(resourceKey, replacement);
            return resourceKey;
        }

        /// <summary>
        /// 获取资源，一般为内容。
        /// </summary>
        /// <typeparam name="TResource">资源类型。</typeparam>
        /// <param name="resourceKey">资源见。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <returns></returns>
        public virtual string GetTemplate<TResource>(string resourceKey, object replacement = null)
        {
            resourceKey = _localizer.GetString(typeof(TResource), resourceKey);
            if (replacement != null)
                resourceKey = ReplaceTemplate(resourceKey, replacement);
            return resourceKey;
        }

        private string ReplaceTemplate(string resourceKey, object fields)
        {
            var replacements = fields.ToDictionary(StringComparer.OrdinalIgnoreCase);
            foreach (var replacement in replacements)
            {
                resourceKey = resourceKey.Replace("{" + replacement.Key.ToLower() + "}", replacement.Value?.ToString());
            }

            return resourceKey;
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
        public virtual bool Save(Email message)
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
        public virtual Task<bool> SaveAsync(Email message)
        {
            if (message.Id > 0)
                return Context.UpdateAsync(message);
            return Context.CreateAsync(message);
        }

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Email.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsExisted(Email message, int expiredSeconds = 300)
        {
            if (message.Id > 0)
                return true;
            var msg = Context.Find(x => x.HashKey == message.HashKey);
            if (msg == null)
                return false;
            return msg.CreatedDate.AddSeconds(expiredSeconds) > DateTimeOffset.Now;
        }

        /// <summary>
        /// 判断消息是否已经存在，用<see cref="Email.HashKey"/>判断。
        /// </summary>
        /// <param name="message">消息实例对象。</param>
        /// <param name="expiredSeconds">过期时间（秒）。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> IsExistedAsync(Email message, int expiredSeconds = 300)
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
        public virtual bool SendEmail(int userId, string emailAddress, string title, string content, Action<Email> action = null)
        {
            var message = new Email();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
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
        public virtual Task<bool> SendEmailAsync(int userId, string emailAddress, string title, string content, Action<Email> action = null)
        {
            var message = new Email();
            message.UserId = userId;
            message.To = emailAddress;
            message.Title = title;
            message.Content = content;
            action?.Invoke(message);
            return SaveAsync(message);
        }

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        public virtual IEnumerable<Email> Load(MessageStatus? status = null)
        {
            var query = Context.AsQueryable().WithNolock();
            if (status != null)
                query.Where(x => x.Status == status);
            query.OrderBy(x => x.Id);
            return query.AsEnumerable(100);
        }

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="status">状态。</param>
        /// <returns>返回消息列表。</returns>
        public virtual Task<IEnumerable<Email>> LoadAsync(MessageStatus? status = null)
        {
            var query = Context.AsQueryable().WithNolock();
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
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : EmailQuery => Context.Load(query);

        /// <summary>
        /// 加载消息列表。
        /// </summary>
        /// <param name="query">消息查询类型。</param>
        /// <returns>返回消息列表。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : EmailQuery => Context.LoadAsync(query);

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

        /// <summary>
        /// 通过Id查询消息。
        /// </summary>
        /// <param name="id">消息id。</param>
        /// <returns>返回消息实例。</returns>
        public virtual Email Find(int id) => Context.Find(id);

        /// <summary>
        /// 通过Id查询消息。
        /// </summary>
        /// <param name="id">消息id。</param>
        /// <returns>返回消息实例。</returns>
        public virtual Task<Email> FindAsync(int id) => Context.FindAsync(id);
    }
}