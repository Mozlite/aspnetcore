﻿using System;
using Mozlite.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Mozlite.Extensions.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Mozlite.Extensions.Extensions.Settings
{
    /// <summary>
    /// 网站配置管理类。
    /// </summary>
    [Suppress(typeof(Mozlite.Extensions.Settings.SettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private readonly IMemoryCache _cache;
        private readonly IDbContext<SettingsAdapter> _db;
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        /// <summary>
        /// 初始化类<see cref="SettingsManager"/>。
        /// </summary>
        /// <param name="context">数据库操作上下文。</param>
        /// <param name="siteContextAccessor">网站访问器接口。</param>
        /// <param name="cache">缓存实例。</param>
        public SettingsManager(IDbContext<SettingsAdapter> context, ISiteContextAccessorBase siteContextAccessor, IMemoryCache cache)
        {
            _db = context;
            _siteContextAccessor = siteContextAccessor;
            _cache = cache;
        }

        private string GetCacheKey(string key, out int siteId)
        {
            siteId = _siteContextAccessor.SiteContext.SiteId;
            return $"sites[{siteId}][{key}]";
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public string GetSettings(string key)
        {
            return _cache.GetOrCreate(GetCacheKey(key, out var siteId), entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                return _db.Find(x => x.SettingKey == key && x.SiteId == siteId)?.SettingValue;
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
            return _cache.GetOrCreate(GetCacheKey(key, out var siteId), entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                var settings = _db.Find(x => x.SettingKey == key && x.SiteId == siteId)?.SettingValue;
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
            return _cache.GetOrCreateAsync(GetCacheKey(key, out var siteId), async entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                var settings = await _db.FindAsync(x => x.SettingKey == key && x.SiteId == siteId);
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
            return _cache.GetOrCreateAsync(GetCacheKey(key, out var siteId), async entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                var settings = await _db.FindAsync(x => x.SettingKey == key && x.SiteId == siteId);
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
        public virtual Task<bool> SaveSettingsAsync<TSiteSettings>(TSiteSettings settings) where TSiteSettings : class, new()
        {
            return SaveSettingsAsync(typeof(TSiteSettings).FullName, settings);
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual Task<bool> SaveSettingsAsync<TSiteSettings>(string key, TSiteSettings settings)
        {
            return SaveSettingsAsync(key, JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual async Task<bool> SaveSettingsAsync(string key, string settings)
        {
            var cacheKey = GetCacheKey(key, out var siteId);
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings, SiteId = siteId };
            if (await _db.AnyAsync(x => x.SettingKey == key && x.SiteId == siteId))
            {
                if (await _db.UpdateAsync(adapter))
                {
                    _cache.Remove(cacheKey);
                    return true;
                }
            }
            return await _db.CreateAsync(adapter);
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
            var cacheKey = GetCacheKey(key, out var siteId);
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings, SiteId = siteId };
            if (_db.Any(x => x.SettingKey == key && x.SiteId == siteId))
            {
                if (_db.Update(adapter))
                {
                    _cache.Remove(cacheKey);
                    return true;
                }
            }
            return _db.Create(adapter);
        }

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public void Refresh(string key)
        {
            _cache.Remove(GetCacheKey(key, out _));
        }

        /// <summary>
        /// 删除网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        public virtual bool DeleteSettings<TSiteSettings>() =>
            DeleteSettings(typeof(TSiteSettings).FullName);

        /// <summary>
        /// 删除网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public virtual bool DeleteSettings(string key)
        {
            if (_db.Delete(key))
            {
                Refresh(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除网站配置实例。
        /// </summary>
        /// <typeparam name="TSiteSettings">网站配置类型。</typeparam>
        public virtual Task<bool> DeleteSettingsAsync<TSiteSettings>() =>
            DeleteSettingsAsync(typeof(TSiteSettings).FullName);

        /// <summary>
        /// 删除网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public virtual async Task<bool> DeleteSettingsAsync(string key)
        {
            if (await _db.DeleteAsync(key))
            {
                Refresh(key);
                return true;
            }
            return false;
        }
    }
}