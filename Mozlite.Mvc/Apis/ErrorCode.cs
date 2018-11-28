namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 错误码。
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 未找到。
        /// </summary>
        NotFound = -8,

        /// <summary>
        /// 发送的数据无效。
        /// </summary>
        InvalidPostData = -7,

        /// <summary>
        /// 令牌无效。
        /// </summary>
        InvalidToken = -6,

        /// <summary>
        /// 验证失败。
        /// </summary>
        AuthorizeFailure = -5,

        /// <summary>
        /// 参数不能为空。
        /// </summary>
        NullParameter = -4,

        /// <summary>
        /// 参数错误。
        /// </summary>
        InvalidParameter = -3,

        /// <summary>
        /// 未知错误。
        /// </summary>
        UnknownError = -2,

        /// <summary>
        /// 失败。
        /// </summary>
        Failured = -1,

        /// <summary>
        /// 成功。
        /// </summary>
        Succeeded = 0,
    }
}