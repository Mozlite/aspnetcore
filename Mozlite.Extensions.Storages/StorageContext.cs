using System;

namespace Mozlite.Extensions.Storages
{
    internal class StorageContext : IStorageContext
    {
        public StorageContext(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        /// <summary>
        /// 缓存键，唯一。
        /// </summary>
        public string CacheKey { get; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        public DateTimeOffset? ExpiredDate { get; private set; }

        /// <summary>
        /// 设置绝对过期时间。
        /// </summary>
        /// <param name="offset">绝对过期时间。</param>
        public void SetAbsoluteExpiredDate(DateTimeOffset offset)
        {
            ExpiredDate = offset;
        }

        /// <summary>
        /// 设置绝对过期时间。
        /// </summary>
        /// <param name="offset">相对于当前时间长度。</param>
        public void SetAbsoluteExpiredDate(TimeSpan offset)
        {
            ExpiredDate = DateTimeOffset.Now.Add(offset);
        }
    }
}