using System;
using Mozlite.Extensions;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 缓存应用程序。
    /// </summary>
    [Target(typeof(Application))]
    public class CacheApplication
    {
        /// <summary>
        /// 应用程序Id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 应用程序Id。
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// 令牌字符串。
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        public DateTimeOffset ExpiredDate { get; set; }

        /// <summary>
        /// 密钥。
        /// </summary>
        public string AppSecret { get; set; }
    }
}