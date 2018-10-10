namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 短信查询类型。
    /// </summary>
    public class SMSQuery : MessageQueryBase
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        protected override MessageType MessageType => MessageType.SMS;
    }
}