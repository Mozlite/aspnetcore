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
        /// 缓存依赖对象值。
        /// </summary>
        public string Dependency { get; private set; }

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
        
        /// <summary>
        /// 设置缓存依赖项。
        /// </summary>
        /// <param name="dependency">依赖项，对象的值。</param>
        public void SetDependency(object dependency)
        {
            Dependency = dependency.ToCacheDependency();
        }
    }
}