namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知状态。
    /// </summary>
    public enum NotificationStatus
    {
        /// <summary>
        /// 新消息。
        /// </summary>
        New,

        /// <summary>
        /// 显示通知，一般用于右下角弹窗消息。
        /// </summary>
        Notified,

        /// <summary>
        /// 已确认。
        /// </summary>
        Confirmed,

        /// <summary>
        /// 已过期。
        /// </summary>
        Expired,
    }
}