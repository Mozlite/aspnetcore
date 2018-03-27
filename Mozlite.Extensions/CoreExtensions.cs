using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 核心扩展类。
    /// </summary>
    public static class CoreExtensions
    {
        /// <summary>
        /// 设置默认缓存时间，3分钟。
        /// </summary>
        /// <param name="cache">缓存实体接口。</param>
        /// <returns>返回缓存实体接口。</returns>
        public static ICacheEntry SetDefaultAbsoluteExpiration(this ICacheEntry cache)
        {
            return cache.SetAbsoluteExpiration(Cores.DefaultCacheExpiration);
        }

    }
}