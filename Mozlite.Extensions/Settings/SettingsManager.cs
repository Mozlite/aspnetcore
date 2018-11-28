using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using System.Threading.Tasks;

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

        /// <summary>
        /// 缓存对象。
        /// </summary>
        protected IMemoryCache Cache { get; }

        /// <summary>
        /// 初始化类<see cref="SettingsManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        /// <param name="cache">缓存接口。</param>
        public SettingsManager(IDbContext<SettingsAdapter> context, IMemoryCache cache)
        {
            Context = context;
            Cache = cache;
        }

        /// <summary>
        /// 获取配置字符串。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <returns>返回当前配置字符串实例。</returns>
        public virtual string GetSettings(string key)
        {
            return Cache.GetOrCreate(key, entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
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
            return Cache.GetOrCreate(key, entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                var settings = Context.Find(x => x.SettingKey == key)?.SettingValue;
                if (settings == null)
                    return new TSiteSettings();
                return Cores.FromJsonString<TSiteSettings>(settings);
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
            return Cache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
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
            return Cache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetDefaultAbsoluteExpiration();
                var settings = await Context.FindAsync(x => x.SettingKey == key);
                if (settings?.SettingValue == null)
                    return new TSiteSettings();
                return Cores.FromJsonString<TSiteSettings>(settings.SettingValue);
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
            return SaveSettingsAsync(key, settings.ToJsonString());
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual async Task<bool> SaveSettingsAsync(string key, string settings)
        {
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings };
            if (await Context.AnyAsync(x => x.SettingKey == key))
            {
                if (await Context.UpdateAsync(adapter))
                {
                    Refresh(key);
                    return true;
                }
            }
            if (await Context.CreateAsync(adapter))
            {
                Refresh(key);
                return true;
            }
            return false;
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
            return SaveSettings(key, settings.ToJsonString());
        }

        /// <summary>
        /// 保存网站配置实例。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        /// <param name="settings">网站配置实例。</param>
        public virtual bool SaveSettings(string key, string settings)
        {
            var adapter = new SettingsAdapter { SettingKey = key, SettingValue = settings };
            if (Context.Any(x => x.SettingKey == key))
            {
                if (Context.Update(adapter))
                {
                    Refresh(key);
                    return true;
                }
            }
            if (Context.Create(adapter))
            {
                Refresh(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        /// <param name="key">配置唯一键。</param>
        public virtual void Refresh(string key)
        {
            Cache.Remove(key);
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
            if (Context.Delete(key))
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
            if (await Context.DeleteAsync(key))
            {
                Refresh(key);
                return true;
            }
            return false;
        }
    }
}