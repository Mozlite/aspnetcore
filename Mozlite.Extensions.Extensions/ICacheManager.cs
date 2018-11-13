using Microsoft.Extensions.Caching.Memory;
using System;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 缓存管理接口，缓存键自动添加当前网站的Id。
    /// </summary>
    public interface ICacheManager : IMemoryCache, ISingletonService
    {

    }

    /// <summary>
    /// 缓存实现类。
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        public CacheManager(IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor)
        {
            _cache = cache;
            _siteContextAccessor = siteContextAccessor;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public virtual void Dispose()
        {
            _cache.Dispose();
        }

        /// <summary>
        /// 尝试获取缓存值。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">返回缓存对象。</param>
        /// <returns>返回获取结果。</returns>
        public virtual bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(CacheKey(key), out value);
        }

        /// <summary>
        /// 创建一个缓存实例。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>缓存实例。</returns>
        public virtual ICacheEntry CreateEntry(object key)
        {
            return _cache.CreateEntry(CacheKey(key));
        }

        /// <summary>
        /// 移除缓存实例。
        /// </summary>
        /// <param name="key">缓存键。</param>
        public virtual void Remove(object key)
        {
            _cache.Remove(CacheKey(key));
        }

        private object CacheKey(object key)
        {
            return new Tuple<int, object>(_siteContextAccessor.SiteContext.SiteId, key);
        }
    }
}