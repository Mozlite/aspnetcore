using System;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 缓存上下文。
    /// </summary>
    public interface IStorageContext
    {
        /// <summary>
        /// 缓存键，唯一。
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        DateTimeOffset? ExpiredDate { get; }

        /// <summary>
        /// 设置绝对过期时间。
        /// </summary>
        /// <param name="offset">绝对过期时间。</param>
        void SetAbsoluteExpiredDate(DateTimeOffset offset);

        /// <summary>
        /// 设置绝对过期时间。
        /// </summary>
        /// <param name="offset">相对于当前时间长度。</param>
        void SetAbsoluteExpiredDate(TimeSpan offset);
    }
}