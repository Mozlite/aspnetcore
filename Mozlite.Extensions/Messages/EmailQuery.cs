namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 电子邮件查询类型。
    /// </summary>
    public class EmailQuery : MessageQueryBase
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        protected override MessageType MessageType => MessageType.Email;
    }
}