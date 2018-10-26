using System;

namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// 令牌结果类型。
    /// </summary>
    public class TokenClientResult : ClientResult<TokenClientResult.Result>
    {
        /// <summary>
        /// 返回数据结果类型。
        /// </summary>
        public class Result
        {
            /// <summary>
            /// 令牌字符串。
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// 过期时间。
            /// </summary>
            public DateTimeOffset ExpiredDate { get; set; }
        }
    }
}