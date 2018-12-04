using Mozlite.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    [Table("apis_Applications")]
    public class Application : ExtendBase
    {
        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [NotUpdated]
        public Guid AppId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        [Size(32)]
        public string Name { get; set; }

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }

        /// <summary>
        /// 应用程序密钥(128位随机字符串)。
        /// </summary>
        [Size(32)]
        public string AppSecret { get; set; } = Cores.GeneralKey(128);

        /// <summary>
        /// 令牌字符串。
        /// </summary>
        [Size(32)]
        [NotUpdated]
        public string Token { get; set; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset ExpiredDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    }
}