using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 存储缓存。
    /// </summary>
    [Table("core_Storages_Caches")]
    public class StorageCache 
    {
        /// <summary>
        /// 缓存键，唯一。
        /// </summary>
        [Key]
        [Size(32)]
        public string CacheKey { get; set; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        public DateTimeOffset? ExpiredDate { get; set; }

        /// <summary>
        /// 缓存依赖对象值。
        /// </summary>
        public string Dependency { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    }
}