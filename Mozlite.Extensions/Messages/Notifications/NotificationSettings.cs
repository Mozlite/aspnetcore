namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知配置。
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// 每个用户保留通知个数。
        /// </summary>
        public int MaxSize { get; set; } = 20;
    }
}