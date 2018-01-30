using System;
using Mozlite.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置管理类。
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        /// <summary>
        /// 数据库操作接口。
        /// </summary>
        protected IDbContext<SettingsAdapter> Context { get; }
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="SettingsManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        public SettingsManager(IDbContext<SettingsAdapter> db, IMemoryCache cache)
        {
            Context = db;
            _cache = cache;
        }
        
        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public virtual string GetSettings(string key)
        {
            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return Context.Find(x => x.SettingKey == key)?.SettingValue;
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回网站配置实例。</returns>
        public virtual TSiteSettings GetSettings<TSiteSettings>(string key) where TSiteSettings : class, new()
        {
            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = Context.Find(x => x.SettingKey == key)?.SettingValue;
                if (settings == null)
                    return new TSiteSettings();
                return JsonConvert.DeserializeObject<TSiteSettings>(settings);
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <returns>返回网站配置实例。</returns>
        public virtual TSiteSettings GetSettings<TSiteSettings>() where TSiteSettings : class, new()
        {
            return GetSettings<TSiteSettings>(typeof(TSiteSettings).FullName);
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public virtual Task<string> GetSettingsAsync(string key)
        {
            return _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = await Context.FindAsync(x => x.SettingKey == key);
                return settings?.SettingValue;
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回网站配置实例。</returns>
        public virtual Task<TSiteSettings> GetSettingsAsync<TSiteSettings>(string key) where TSiteSettings : class, new()
        {
            return _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = await Context.FindAsync(x => x.SettingKey == key);
                if (settings?.SettingValue == null)
                    return new TSiteSettings();
                return JsonConvert.DeserializeObject<TSiteSettings>(settings.SettingValue);
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <returns>返回网站配置实例。</returns>
        public virtual Task<TSiteSettings> GetSettingsAsync<TSiteSettings>() where TSiteSettings : class, new()
        {
            return GetSettingsAsync<TSiteSettings>(typeof(TSiteSettings).FullName);
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="settings">网站配置实例。</param>
        public virtual bool SaveSettings<TSiteSettings>(TSiteSettings settings) where TSiteSettings : class, new()
        {
            return SaveSettings(typeof(TSiteSettings).FullName, settings);
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual bool SaveSettings<TSiteSettings>(string key, TSiteSettings settings)
        {
            return SaveSettings(key, JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual bool SaveSettings(string key, string settings)
        {
            var cacheKey = key;
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings };
            if (Context.Any(x => x.SettingKey == key))
            {
                if (Context.Update(adapter))
                {
                    _cache.Remove(cacheKey);
                    return true;
                }
            }
            return Context.Create(adapter);
        }

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public virtual void Refresh(string key)
        {
            _cache.Remove(key);
        }
    }
}