namespace Mozlite.Mvc.Messages
{
    /// <summary>
    /// 消息类型。
    /// </summary>
    public class BsMessage
    {
        /// <summary>
        /// 初始化类<see cref="BsMessage"/>。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <param name="type">消息类型。</param>
        /// <param name="redirectUrl">转向地址。</param>
        public BsMessage(string message, BsType? type = null, string redirectUrl = null)
        {
            Type = type;
            Message = message;
            RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// 消息类型。
        /// </summary>
        public BsType? Type { get; }

        /// <summary>
        /// 消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 转向地址。
        /// </summary>
        public string RedirectUrl { get; }
    }
}