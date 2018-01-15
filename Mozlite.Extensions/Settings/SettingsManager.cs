using System;
using Mozlite.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Mozlite.Extensions.Sites;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置管理类。
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        private readonly IRepository<SettingsAdapter> _repository;
        private readonly ISiteManager _siteManager;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="SettingsManager"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="siteManager">网站配置管理。</param>
        /// <param name="cache">缓存接口。</param>
        public SettingsManager(IRepository<SettingsAdapter> repository, ISiteManager siteManager, IMemoryCache cache)
        {
            _repository = repository;
            _siteManager = siteManager;
            _cache = cache;
        }

        private string GetCacheKey(string key, out int settingId)
        {
            settingId = _siteManager.GetSite().SiteId;
            return $"settings[{settingId}][{key}]";
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public string GetSettings(string key)
        {
            return _cache.GetOrCreate(GetCacheKey(key, out var settingId), entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return _repository.Find(x => x.SettingKey == key && x.SettingId == settingId)?.SettingValue;
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回网站配置实例。</returns>
        public TSiteSettings GetSettings<TSiteSettings>(string key) where TSiteSettings : class, new()
        {
            return _cache.GetOrCreate(GetCacheKey(key, out var settingId), entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = _repository.Find(x => x.SettingKey == key && x.SettingId == settingId)?.SettingValue;
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
        public TSiteSettings GetSettings<TSiteSettings>() where TSiteSettings : class, new()
        {
            return GetSettings<TSiteSettings>(typeof(TSiteSettings).FullName);
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public Task<string> GetSettingsAsync(string key)
        {
            return _cache.GetOrCreateAsync(GetCacheKey(key, out var settingId), async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = await _repository.FindAsync(x => x.SettingKey == key && x.SettingId == settingId);
                return settings?.SettingValue;
            });
        }

        /// <summary>
        /// 获取网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回网站配置实例。</returns>
        public Task<TSiteSettings> GetSettingsAsync<TSiteSettings>(string key) where TSiteSettings : class, new()
        {
            return _cache.GetOrCreateAsync(GetCacheKey(key, out var settingId), async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = await _repository.FindAsync(x => x.SettingKey == key && x.SettingId == settingId);
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
        public Task<TSiteSettings> GetSettingsAsync<TSiteSettings>() where TSiteSettings : class, new()
        {
            return GetSettingsAsync<TSiteSettings>(typeof(TSiteSettings).FullName);
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="settings">网站配置实例。</param>
        public bool SaveSettings<TSiteSettings>(TSiteSettings settings) where TSiteSettings : class, new()
        {
            return SaveSettings(typeof(TSiteSettings).FullName, settings);
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public bool SaveSettings<TSiteSettings>(string key, TSiteSettings settings)
        {
            return SaveSettings(key, JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public bool SaveSettings(string key, string settings)
        {
            var cacheKey = GetCacheKey(key, out var settingId);
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings, SettingId = settingId };
            if (_repository.Any(x => x.SettingKey == key && x.SettingId == settingId))
            {
                if (_repository.Update(adapter))
                {
                    _cache.Remove(cacheKey);
                    return true;
                }
            }
            return _repository.Create(adapter);
        }

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public void Refresh(string key)
        {
            _cache.Remove(GetCacheKey(key, out _));
        }
    }
}