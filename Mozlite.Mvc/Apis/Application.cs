using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mozlite.Extensions;
using Mozlite.Extensions.Data;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    [Table("apis_Applications")]
    public class Application : ExtendBase, IIdObject<Guid>
    {
        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [NotMapped]
        public Guid AppId { get => Id; set => Id = value; }

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        [Size(32)]
        public string Name { get; set; }

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
        public DateTime ExpiredDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}