using Mozlite.Mvc.Properties;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API结果。
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 成功。
        /// </summary>
        public static readonly ApiResult Succeeded = new ApiResult(ErrorCode.Succeeded);

        /// <summary>
        /// 失败。
        /// </summary>
        public static readonly ApiResult Failured = new ApiResult(ErrorCode.Failured);

        /// <summary>
        /// 未知错误。
        /// </summary>
        public static readonly ApiResult UnknownError = new ApiResult(ErrorCode.UnknownError);

        /// <summary>
        /// 初始化类<see cref="ApiResult"/>。
        /// </summary>
        /// <param name="code">错误码。</param>
        public ApiResult(ErrorCode code)
        {
            Code = code;
            Msg = Resources.ResourceManager.GetString($"ErrorCode_{code}");
        }

        /// <summary>
        /// 初始化类<see cref="ApiResult"/>。
        /// </summary>
        /// <param name="code">错误码。</param>
        /// <param name="msg">消息。</param>
        public ApiResult(ErrorCode code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        /// <summary>
        /// 错误码。
        /// </summary>
        public ErrorCode Code { get; }

        /// <summary>
        /// 消息。
        /// </summary>
        public string Msg { get; private set; }

        /// <summary>
        /// 返回错误信息。
        /// </summary>
        /// <returns>返回错误信息。</returns>
        public override string ToString()
        {
            return Msg;
        }

        /// <summary>
        /// 隐式转换失败消息。
        /// </summary>
        /// <param name="msg">失败消息。</param>
        public static implicit operator ApiResult(string msg) => new ApiResult(ErrorCode.Failured, msg);

        internal ApiResult Format(params object[] args)
        {
            if (Msg != null)
                Msg = string.Format(Msg, args);
            return this;
        }
    }
}