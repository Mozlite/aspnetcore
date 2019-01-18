using System;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知类型。
    /// </summary>
    public class Notifier : INotifier
    {
        private readonly INotificationManager _notificationManager;
        private readonly INotificationTypeManager _typeManager;
        /// <summary>
        /// 初始化类<see cref="Notifier"/>。
        /// </summary>
        /// <param name="notificationManager">通知管理接口。</param>
        /// <param name="typeManager">通知类型管理接口。</param>
        public Notifier(INotificationManager notificationManager, INotificationTypeManager typeManager)
        {
            _notificationManager = notificationManager;
            _typeManager = typeManager;
        }

        /// <summary>
        /// 实例化一个通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        /// <returns>返回通知实例。</returns>
        protected virtual Notification Create(int userId, int typeId, string message, params object[] args)
        {
            var notification = new Notification();
            notification.UserId = userId;
            notification.TypeId = typeId;
            notification.Message = string.Format(message, args);
            return notification;
        }

        /// <summary>
        /// 通过类型名称获取类型Id。
        /// </summary>
        /// <param name="typeName">类型名称。</param>
        /// <returns>返回类型Id。</returns>
        protected int GetTypeId(string typeName) => _typeManager.Find(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual void Send(int userId, int typeId, string message, params object[] args)
        {
            var notification = Create(userId, typeId, message, args);
            _notificationManager.Save(notification);
        }

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual Task SendAsync(int userId, int typeId, string message, params object[] args)
        {
            var notification = Create(userId, typeId, message, args);
            return _notificationManager.SaveAsync(notification);
        }

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual void Send(int userId, string typeName, string message, params object[] args) =>
            Send(userId, GetTypeId(typeName), message, args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual Task SendAsync(int userId, string typeName, string message, params object[] args) =>
            SendAsync(userId, GetTypeId(typeName), message, args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual void Send(int[] userId, int typeId, string message, params object[] args)
        {
            var notification = Create(0, typeId, message, args);
            _notificationManager.Save(userId, notification);
        }

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual Task SendAsync(int[] userId, int typeId, string message, params object[] args)
        {
            var notification = Create(0, typeId, message, args);
            return _notificationManager.SaveAsync(userId, notification);
        }

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual void Send(int[] userId, string typeName, string message, params object[] args) =>
            Send(userId, GetTypeId(typeName), message, args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        public virtual Task SendAsync(int[] userId, string typeName, string message, params object[] args) =>
            SendAsync(userId, GetTypeId(typeName), message, args);
    }
}