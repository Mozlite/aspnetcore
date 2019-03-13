using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Groups;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 字典管理实现类。
    /// </summary>
    public class SettingDictionaryManager : GroupManager<SettingDictionary>, ISettingDictionaryManager
    {
        private static readonly object _pathCacheKey = new Tuple<Type, string>(typeof(SettingDictionary), "path");
        /// <summary>
        /// 初始化类<see cref="SettingDictionaryManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        public SettingDictionaryManager(IDbContext<SettingDictionary> context, IMemoryCache cache)
            : base(context, cache)
        {
        }

        private ConcurrentDictionary<string, SettingDictionary> LoadPathCache()
        {
            return Cache.GetOrCreate(_pathCacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var settings = Fetch().ToDictionary(x => x.Path, StringComparer.OrdinalIgnoreCase);
                return new ConcurrentDictionary<string, SettingDictionary>(settings);
            });
        }

        private Task<ConcurrentDictionary<string, SettingDictionary>> LoadPathCacheAsync()
        {
            return Cache.GetOrCreateAsync(_pathCacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var settings = (await FetchAsync()).ToDictionary(x => x.Path, StringComparer.OrdinalIgnoreCase);
                return new ConcurrentDictionary<string, SettingDictionary>(settings);
            });
        }

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            Cache.Remove(_pathCacheKey);
        }

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        public virtual string GetSettings(string path)
        {
            var settings = LoadPathCache();
            settings.TryGetValue(path, out var value);
            return value;
        }

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        public virtual async Task<string> GetSettingsAsync(string path)
        {
            var settings = await LoadPathCacheAsync();
            settings.TryGetValue(path, out var value);
            return value;
        }

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        public virtual string GetOrAddSettings(string path)
        {
            var settings = LoadPathCache();
            if (settings.TryGetValue(path, out var setting))
                return setting;
            if (Context.BeginTransaction(db =>
            {
                var names = path.Split('.');
                var parentId = 0;
                foreach (var name in names)
                {
                    setting = Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (setting == null)
                    {
                        setting = new SettingDictionary { ParentId = parentId, Name = name, Value = name };
                        if (db.Create(setting))
                            parentId = setting.Id;
                        else
                            return false;
                    }
                    else
                    {
                        parentId = setting.Id;
                    }
                }

                return true;
            }))
                return setting;
            return null;
        }

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        public virtual async Task<string> GetOrAddSettingsAsync(string path)
        {
            var settings = await LoadPathCacheAsync();
            if (settings.TryGetValue(path, out var setting))
                return setting;
            if (await Context.BeginTransactionAsync(async db =>
            {
                var names = path.Split('.');
                var parentId = 0;
                foreach (var name in names)
                {
                    setting = await FindAsync(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (setting == null)
                    {
                        setting = new SettingDictionary { ParentId = parentId, Name = name, Value = name };
                        if (await db.CreateAsync(setting))
                            parentId = setting.Id;
                        else
                            return false;
                    }
                    else
                    {
                        parentId = setting.Id;
                    }
                }

                return true;
            }))
                return setting;
            return null;
        }
    }
}