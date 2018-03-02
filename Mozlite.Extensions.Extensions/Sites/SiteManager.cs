using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站管理类。
    /// </summary>
    public class SiteManager : ISiteManager
    {
        private readonly IDbContext<SiteAdapter> _sdb;
        private readonly IDbContext<SiteDomain> _sddb;
        private readonly IMemoryCache _cache;
        private readonly IEnumerable<ISitableCreatedHandler> _handlers;

        /// <summary>
        /// <see cref="SiteManager"/>。
        /// </summary>
        /// <param name="sdb">网站数据库操作。</param>
        /// <param name="sddb">域名数据库操作。</param>
        /// <param name="cache">缓存数据库操作。</param>
        /// <param name="handlers">处理方法列表。</param>
        public SiteManager(IDbContext<SiteAdapter> sdb, IDbContext<SiteDomain> sddb, IMemoryCache cache, IEnumerable<ISitableCreatedHandler> handlers)
        {
            _sdb = sdb;
            _sddb = sddb;
            _cache = cache;
            _handlers = handlers;
        }

        private IDictionary<string, SiteDomain> LoadCacheDomains()
        {
            return _cache.GetOrCreate(typeof(SiteDomain), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return _sddb.Fetch()
                    .OrderByDescending(x => x.Domain.Length)
                    .ToDictionary(x => x.Domain, StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, SiteDomain>> LoadCacheDomainsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(SiteDomain), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var domains = await _sddb.FetchAsync();
                return domains.OrderByDescending(x => x.Domain.Length)
                    .ToDictionary(x => x.Domain, StringComparer.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        public virtual bool SetDefault(int siteId, string domain)
        {
            if (_sddb.BeginTransaction(db =>
            {
                db.Update(x => x.SiteId == siteId, new { IsDefault = false });
                db.Update(x => x.SiteId == siteId && x.Domain == domain, new { IsDefault = true });
                return true;
            }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        public virtual async Task<bool> SetDefaultAsync(int siteId, string domain)
        {
            if (await _sddb.BeginTransactionAsync(async db =>
            {
                await db.UpdateAsync(x => x.SiteId == siteId, new { IsDefault = false });
                await db.UpdateAsync(x => x.SiteId == siteId && x.Domain == domain, new { IsDefault = true });
                return true;
            }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        public virtual bool SetDisabled(int siteId, string domain, bool disabled = true)
        {
            if (_sddb.Update(x => x.SiteId == siteId && x.Domain == domain, new { Disabled = disabled }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        public virtual async Task<bool> SetDisabledAsync(int siteId, string domain, bool disabled = true)
        {
            if (await _sddb.UpdateAsync(x => x.SiteId == siteId && x.Domain == domain, new { Disabled = disabled }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public virtual IEnumerable<SiteBase> LoadSites()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SiteId, x.SiteName, x.SiteKey, x.UpdatedDate })
                .AsEnumerable(reader => new SiteBase
                {
                    SiteId = reader.GetInt32(0),
                    SiteName = reader.GetString(1),
                    SiteKey = reader.GetString(2),
                    UpdatedDate = reader.GetFieldValue<DateTimeOffset>(3)
                });
        }

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        public virtual IEnumerable<SiteDomain> LoadDomains()
        {
            return LoadCacheDomains().Values;
        }

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsValid(string domain)
        {
            if (domain.Equals("localhost", StringComparison.OrdinalIgnoreCase) || domain.Equals("127.0.0.1"))
                return true;
            return LoadCacheDomains().ContainsKey(domain);
        }

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public virtual DataResult Create(SiteDomain domain)
        {
            if (IsValid(domain.Domain))
                return DataAction.Created;
            if (_sddb.Create(domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Created;
            }
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public virtual DataResult Delete(int siteId, string domain)
        {
            if (_sddb.Delete(x => x.SiteId == siteId && x.Domain == domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Deleted;
            }
            return DataAction.DeletedFailured;
        }

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回当前网站域名实例。</returns>
        public virtual SiteDomain GetDomain(string domain)
        {
            LoadCacheDomains().TryGetValue(domain, out var site);
            return site;
        }

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回当前网站域名实例。</returns>
        public virtual async Task<SiteDomain> GetDomainAsync(string domain)
        {
            var sites = await LoadCacheDomainsAsync();
            sites.TryGetValue(domain, out var site);
            return site;
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public virtual Task<IEnumerable<SiteBase>> LoadSitesAsync()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SiteId, x.SiteName, x.SiteKey, x.UpdatedDate })
                .AsEnumerableAsync(reader => new SiteBase
                {
                    SiteId = reader.GetInt32(0),
                    SiteName = reader.GetString(1),
                    SiteKey = reader.GetString(2),
                    UpdatedDate = reader.GetFieldValue<DateTimeOffset>(3)
                });
        }

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        public virtual async Task<IEnumerable<SiteDomain>> LoadDomainsAsync()
        {
            var domains = await LoadCacheDomainsAsync();
            return domains.Values;
        }

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        public virtual async Task<bool> IsValidAsync(string domain)
        {
            if (domain.Equals("localhost", StringComparison.OrdinalIgnoreCase) || domain.Equals("127.0.0.1"))
                return true;
            var domains = await LoadCacheDomainsAsync();
            return domains.ContainsKey(domain);
        }

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<DataResult> CreateAsync(SiteDomain domain)
        {
            if (await IsValidAsync(domain.Domain))
                return DataAction.Created;
            if (await _sddb.CreateAsync(domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Created;
            }
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(int siteId, string domain)
        {
            if (await _sddb.DeleteAsync(x => x.SiteId == siteId && x.Domain == domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Deleted;
            }
            return DataAction.DeletedFailured;
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public virtual DataResult Save(SiteBase site)
        {
            var adapter = CreateInstance(site);
            if (adapter.SiteId > 0)
                return DataResult.FromResult(_sdb.Update(adapter), DataAction.Updated);
            if (IsDuplicated(site))
                return DataResult.FromResult(_sdb.Update(x => x.SiteKey == adapter.SiteKey, new { adapter.SiteName, adapter.UpdatedDate, adapter.SettingValue }), DataAction.Updated);
            return DataResult.FromResult(_sdb.BeginTransaction(db =>
            {
                if (!db.Create(adapter))
                    return false;
                site.SiteId = adapter.SiteId;
                foreach (var handler in _handlers)
                {
                    handler.OnCreated(site);
                }
                return true;
            }), DataAction.Created);
        }

        /// <summary>
        /// 获取网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="key">网站唯一键。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual TSite GetSiteByKey<TSite>(string key) where TSite : SiteBase, new()
        {
            var settings = _sdb.Find(x => x.SiteKey == key);
            return CreateInstance<TSite>(settings);
        }

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual TSite GetSite<TSite>(string domain) where TSite : SiteBase, new()
        {
            if (LoadCacheDomains().TryGetValue(domain, out var site))
            {
                return GetSite<TSite>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual TSite GetSite<TSite>(int siteId) where TSite : SiteBase, new()
        {
            var settings = _sdb.Find(siteId);
            return CreateInstance<TSite>(settings);
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public virtual async Task<DataResult> SaveAsync(SiteBase site)
        {
            var adapter = CreateInstance(site);
            if (adapter.SiteId > 0)
                return DataResult.FromResult(await _sdb.UpdateAsync(adapter), DataAction.Updated);
            if (await IsDuplicatedAsync(site))
                return DataResult.FromResult(await _sdb.UpdateAsync(x => x.SiteKey == adapter.SiteKey, new { adapter.SiteName, adapter.UpdatedDate, adapter.SettingValue }), DataAction.Updated);
            return DataResult.FromResult(await _sdb.BeginTransactionAsync(async db =>
            {
                if (!await db.CreateAsync(adapter))
                    return false;
                site.SiteId = adapter.SiteId;
                foreach (var handler in _handlers)
                {
                    await handler.OnCreatedAsync(site);
                }
                return true;
            }), DataAction.Created);
        }

        /// <summary>
        /// 获取网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="key">网站唯一键。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual async Task<TSite> GetSiteByKeyAsync<TSite>(string key) where TSite : SiteBase, new()
        {
            var settings = await _sdb.FindAsync(x => x.SiteKey == key);
            return CreateInstance<TSite>(settings);
        }

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual async Task<TSite> GetSiteAsync<TSite>(string domain) where TSite : SiteBase, new()
        {
            var sites = await LoadCacheDomainsAsync();
            if (sites.TryGetValue(domain, out var site))
            {
                return await GetSiteAsync<TSite>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public virtual async Task<TSite> GetSiteAsync<TSite>(int siteId) where TSite : SiteBase, new()
        {
            var settings = await _sdb.FindAsync(siteId);
            return CreateInstance<TSite>(settings);
        }

        /// <summary>
        /// 实例化网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="site">网站实例。</param>
        /// <returns>返回网站实例。</returns>
        protected SiteAdapter CreateInstance<TSite>(TSite site) where TSite : SiteBase, new()
        {
            var adapter = new SiteAdapter();
            adapter.SettingValue = JsonConvert.SerializeObject(site);
            adapter.SiteKey = site.SiteKey;
            adapter.SiteId = site.SiteId;
            adapter.SiteName = site.SiteName;
            return adapter;
        }

        /// <summary>
        /// 实例化网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="adapter">网站适配实例。</param>
        /// <returns>返回网站实例。</returns>
        protected TSite CreateInstance<TSite>(SiteAdapter adapter) where TSite : SiteBase, new()
        {
            if (adapter == null) return null;
            TSite site;
            if (string.IsNullOrWhiteSpace(adapter.SettingValue))
                site = new TSite();
            else
                site = JsonConvert.DeserializeObject<TSite>(adapter.SettingValue);
            site.SiteName = adapter.SiteName;
            site.UpdatedDate = adapter.UpdatedDate;
            site.SiteId = adapter.SiteId;
            return site;
        }

        /// <summary>
        /// 判断是否已经存在，唯一键重复。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsDuplicated(SiteBase site)
        {
            return _sdb.Any(x => x.SiteId != site.SiteId && x.SiteKey == site.SiteKey);
        }

        /// <summary>
        /// 判断是否已经存在，唯一键重复。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回判断结果。</returns>
        public virtual Task<bool> IsDuplicatedAsync(SiteBase site)
        {
            return _sdb.AnyAsync(x => x.SiteId != site.SiteId && x.SiteKey == site.SiteKey);
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回所有网站。</returns>
        public virtual IEnumerable<TSite> LoadSites<TSite>() where TSite : SiteBase, new()
        {
            return _sdb.Fetch().Select(CreateInstance<TSite>);
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回所有网站。</returns>
        public virtual async Task<IEnumerable<TSite>> LoadSitesAsync<TSite>() where TSite : SiteBase, new()
        {
            var sites = await _sdb.FetchAsync();
            return sites.Select(CreateInstance<TSite>);
        }
    }
}