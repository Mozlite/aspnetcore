namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 消息查询类型。
    /// </summary>
    public class MessageQuery : MessageQueryBase
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        protected override MessageType MessageType => MessageType.Message;
    }
}