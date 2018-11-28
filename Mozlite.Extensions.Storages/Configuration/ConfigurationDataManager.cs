using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Configuration
{
    /// <summary>
    /// 配置管理。
    /// </summary>
    public class ConfigurationDataManager : IConfigurationDataManager
    {
        private readonly IMemoryCache _cache;
        private const string ConfigDir = "configdata";

        private string GetPath(string name)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigDir);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return Path.Combine(path, $"{name}.json");
        }

        private string GetCacheKey(string name) => $"{ConfigDir}:[{name}]";

        /// <summary>
        /// 初始化类<see cref="ConfigurationDataManager"/>。
        /// </summary>
        /// <param name="cache">缓存接口。</param>
        public ConfigurationDataManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 加载配置。   
        /// </summary>
        /// <typeparam name="TConfiguration">配置类型。</typeparam>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="minutes">缓存分钟数。</param>
        /// <returns>返回配置实例。</returns>
        public virtual TConfiguration LoadConfiguration<TConfiguration>(string name, int minutes = 3)
        {
            if (minutes <= 0) return LoadConfiguration<TConfiguration>(name);
            return _cache.GetOrCreate(GetCacheKey(name), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes));
                return LoadConfiguration<TConfiguration>(name);
            });
        }

        private TConfiguration LoadConfiguration<TConfiguration>(string name)
        {
            var path = GetPath(name);
            if (!File.Exists(path))
                return default;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
                return Cores.FromJsonString<TConfiguration>(sr.ReadToEnd());
        }

        /// <summary>
        /// 加载配置。   
        /// </summary>
        /// <typeparam name="TConfiguration">配置类型。</typeparam>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="minutes">缓存分钟数。</param>
        /// <returns>返回配置实例。</returns>
        public virtual async Task<TConfiguration> LoadConfigurationAsync<TConfiguration>(string name, int minutes = 3)
        {
            if (minutes <= 0) return await LoadConfigurationAsync<TConfiguration>(name);
            return await _cache.GetOrCreateAsync(GetCacheKey(name), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes));
                return await LoadConfigurationAsync<TConfiguration>(name);
            });
        }

        private async Task<TConfiguration> LoadConfigurationAsync<TConfiguration>(string name)
        {
            var path = GetPath(name);
            if (!File.Exists(path))
                return default;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
                return Cores.FromJsonString<TConfiguration>(await sr.ReadToEndAsync());
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="configuration">配置实例。</param>
        public virtual void SaveConfiguration(string name, object configuration)
        {
            var path = GetPath(name);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                sw.Write(configuration.ToJsonString());
            _cache.Remove(GetCacheKey(name));
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="configuration">配置实例。</param>
        public virtual async Task SaveConfigurationAsync(string name, object configuration)
        {
            var path = GetPath(name);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                await sw.WriteAsync(configuration.ToJsonString());
            _cache.Remove(GetCacheKey(name));
        }
    }
}