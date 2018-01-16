using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储缓存实现类。
    /// </summary>
    public class FileStorageCache : IStorageCache
    {
        private readonly IMemoryCache _cache;
        private readonly IStorageDirectory _root;
        private readonly IRepository<StorageCache> _repository;
        private readonly string _cacheRoot;
        public FileStorageCache(IMemoryCache cache, IStorageDirectory root, IRepository<StorageCache> repository)
        {
            _cache = cache;
            _root = root;
            _repository = repository;
            _cacheRoot = root.GetPhysicalPath("caches");
            if (!Directory.Exists(_cacheRoot)) Directory.CreateDirectory(_cacheRoot);
        }

        /// <summary>
        /// 获取缓存键。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>返回缓存键。</returns>
        protected string GetCacheKey(object key)
        {
            return Cores.Md5($"moz::storages::caches[{key.GetType()}::{key}]");
        }

        /// <summary>
        /// 获取当前缓存文件物理路径。
        /// </summary>
        /// <param name="hashedKey">当前缓存键的哈希值。</param>
        /// <param name="ensureDir">如果文件夹不存在，添加文件夹。</param>
        /// <returns>返回当前缓存文件的物理路径。</returns>
        protected string GetFilePath(string hashedKey, bool ensureDir = false)
        {
            var path = Path.Combine(_cacheRoot, hashedKey.MakedPath());
            if (ensureDir)
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            return path;
        }

        /// <summary>
        /// 获取缓存配置实例。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <returns>返回缓存配置实例。</returns>
        protected StorageCache GetCache(object key, object dependency)
        {
            var hashedKey = GetCacheKey(key);
            var cache = _cache.GetOrCreate(hashedKey, ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return _repository.Find(hashedKey);
            });
            if (cache == null) return null;
            if (cache.Dependency != dependency.ToCacheDependency() ||
                cache.ExpiredDate != null && cache.ExpiredDate < DateTimeOffset.Now)
            {
                Remove(hashedKey);
                return null;
            }
            return cache;
        }

        /// <summary>
        /// 获取缓存配置实例。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <returns>返回缓存配置实例。</returns>
        protected async Task<StorageCache> GetCacheAsync(object key, object dependency)
        {
            var hashedKey = GetCacheKey(key);
            var cache = await _cache.GetOrCreateAsync(hashedKey, async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return await _repository.FindAsync(hashedKey);
            });
            if (cache == null) return null;
            if (cache.Dependency != dependency.ToCacheDependency() ||
                cache.ExpiredDate != null && cache.ExpiredDate < DateTimeOffset.Now)
            {
                await RemoveAsync(hashedKey);
                return null;
            }
            return cache;
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public string GetOrCreate(object key, Func<IStorageContext, string> action)
        {
            return GetOrCreate(key, null, action);
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public virtual string GetOrCreate(object key, object dependency, Func<IStorageContext, string> action)
        {
            var cache = GetCache(key, dependency);
            if (cache == null)
            {
                var cacheKey = GetCacheKey(key);
                var context = new StorageContext(cacheKey);
                var text = action(context);
                cache = new StorageCache
                {
                    CacheKey = cacheKey,
                    Dependency = context.Dependency,
                    ExpiredDate = context.ExpiredDate
                };
                _repository.Create(cache);
                using (var fs = new FileStream(GetFilePath(cacheKey, true), FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(text);
                }
                return text;
            }
            var path = GetFilePath(cache.CacheKey);
            if (!File.Exists(path))//缓存文件不存在
            {
                Remove(cache.CacheKey);
                return GetOrCreate(key, dependency, action);
            }
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public Task<string> GetOrCreateAsync(object key, Func<IStorageContext, Task<string>> action)
        {
            return GetOrCreateAsync(key, null, action);
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public virtual async Task<string> GetOrCreateAsync(object key, object dependency, Func<IStorageContext, Task<string>> action)
        {
            var cache = await GetCacheAsync(key, dependency);
            if (cache == null)
            {
                var cacheKey = GetCacheKey(key);
                var context = new StorageContext(cacheKey);
                var text = await action(context);
                cache = new StorageCache
                {
                    CacheKey = cacheKey,
                    Dependency = context.Dependency,
                    ExpiredDate = context.ExpiredDate
                };
                await _repository.CreateAsync(cache);
                using (var fs = new FileStream(GetFilePath(cacheKey, true), FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(text);
                }
                return text;
            }
            var path = GetFilePath(cache.CacheKey);
            if (!File.Exists(path))//缓存文件不存在
            {
                await RemoveAsync(cache.CacheKey);
                return await GetOrCreateAsync(key, dependency, action);
            }
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public TCache GetOrCreate<TCache>(object key, Func<IStorageContext, TCache> action)
        {
            return GetOrCreate(key, null, action);
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public TCache GetOrCreate<TCache>(object key, object dependency, Func<IStorageContext, TCache> action)
        {
            var cache = GetCache(key, dependency);
            if (cache == null)
            {
                var cacheKey = GetCacheKey(key);
                var context = new StorageContext(cacheKey);
                var text = action(context);
                cache = new StorageCache
                {
                    CacheKey = cacheKey,
                    Dependency = context.Dependency,
                    ExpiredDate = context.ExpiredDate
                };
                _repository.Create(cache);
                using (var fs = new FileStream(GetFilePath(cacheKey, true), FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(text));
                }
                return text;
            }
            var path = GetFilePath(cache.CacheKey);
            if (!File.Exists(path))//缓存文件不存在
            {
                Remove(cache.CacheKey);
                return GetOrCreate(key, dependency, action);
            }
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            {
                var text = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TCache>(text);
            }
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public Task<TCache> GetOrCreateAsync<TCache>(object key, Func<IStorageContext, Task<TCache>> action)
        {
            return GetOrCreateAsync(key, null, action);
        }

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        public virtual async Task<TCache> GetOrCreateAsync<TCache>(object key, object dependency, Func<IStorageContext, Task<TCache>> action)
        {
            var cache = await GetCacheAsync(key, dependency);
            if (cache == null)
            {
                var cacheKey = GetCacheKey(key);
                var context = new StorageContext(cacheKey);
                var text = await action(context);
                cache = new StorageCache
                {
                    CacheKey = cacheKey,
                    Dependency = context.Dependency,
                    ExpiredDate = context.ExpiredDate
                };
                await _repository.CreateAsync(cache);
                using (var fs = new FileStream(GetFilePath(cacheKey, true), FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(text);
                }
                return text;
            }
            var path = GetFilePath(cache.CacheKey);
            if (!File.Exists(path))//缓存文件不存在
            {
                await RemoveAsync(cache.CacheKey);
                return await GetOrCreateAsync(key, dependency, action);
            }
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            {
                var text = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<TCache>(text);
            }
        }

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        public void Remove(object key)
        {
            var cacheKey = GetCacheKey(key);
            Remove(cacheKey);
        }

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        public Task RemoveAsync(object key)
        {
            var cacheKey = GetCacheKey(key);
            return RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="hashedKey">缓存唯一键。</param>
        protected void Remove(string hashedKey)
        {
            //删除文件
            var path = GetFilePath(hashedKey);
            if (File.Exists(path))
                File.Delete(path);
            //删除数据库
            _repository.Delete(hashedKey);
            //删除缓存
            _cache.Remove(hashedKey);
        }

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="hashedKey">缓存唯一键。</param>
        protected async Task RemoveAsync(string hashedKey)
        {
            //删除文件
            var path = GetFilePath(hashedKey);
            if (File.Exists(path))
                File.Delete(path);
            //删除数据库
            await _repository.DeleteAsync(hashedKey);
            //删除缓存
            _cache.Remove(hashedKey);
        }
    }
}