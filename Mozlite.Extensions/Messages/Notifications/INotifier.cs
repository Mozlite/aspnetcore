using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知接口。
    /// </summary>
    public interface INotifier : ISingletonService
    {
        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        void Send(int userId, int typeId, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        Task SendAsync(int userId, int typeId, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        void Send(int userId, string typeName, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        Task SendAsync(int userId, string typeName, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        void Send(int[] userId, int typeId, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeId">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        Task SendAsync(int[] userId, int typeId, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        void Send(int[] userId, string typeName, string message, params object[] args);

        /// <summary>
        /// 发送通知。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="typeName">通知类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args">消息内容参数。</param>
        Task SendAsync(int[] userId, string typeName, string message, params object[] args);
    }
}
