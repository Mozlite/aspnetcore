using System;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 网站配置管理类。
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        private readonly IRepository<SettingsAdapter> _repository;
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 初始化类<see cref="SettingsManager"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        public SettingsManager(IRepository<SettingsAdapter> repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public string GetSettings(string key)
        {
            return _cache.GetOrCreate($"sitesettings[{key}]", entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return _repository.Find(x=>x.SettingsId == key)?.SettingsValue;
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
            return _cache.GetOrCreate($"sitesettings[{key}]", entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var settings = _repository.Find(x => x.SettingsId == key)?.SettingsValue;
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
            var adapter = new SettingsAdapter { SettingsId = key, SettingsValue = settings };
            if (_repository.Any(x=>x.SettingsId == key))
            {
                if (_repository.Update(adapter))
                {
                    Refresh(key);
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
            _cache.Remove($"sitesettings[{key}]");
        }
    }
}