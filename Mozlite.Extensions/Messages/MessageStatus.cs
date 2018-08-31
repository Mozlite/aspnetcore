namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 消息状态。
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// 等待发送，系统消息未读。
        /// </summary>
        Pending,

        /// <summary>
        /// 发送成功，系统消息已读。
        /// </summary>
        Completed,

        /// <summary>
        /// 发送失败。
        /// </summary>
        Failured,
    }
}