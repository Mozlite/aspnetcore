namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 上传/下载结果。
    /// </summary>
    public class MediaResult
    {
        /// <summary>
        /// 初始化类<see cref="MediaResult"/>。
        /// </summary>
        /// <param name="url">文件访问的URL地址。</param>
        /// <param name="message">错误消息。</param>
        internal MediaResult(string url, string message = null)
        {
            Url = url;
            Message = message;
            Succeeded = message == null;
        }

        /// <summary>
        /// 文件访问的URL地址。
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 错误消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 是否成功。
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// 隐式将字符串转换为上传/下载结果实例。
        /// </summary>
        /// <param name="message">错误消息。</param>
        public static implicit operator MediaResult(string message) => new MediaResult(null, message);
    }
}