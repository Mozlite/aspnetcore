using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions;
using Mozlite.Extensions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序管理。
    /// </summary>
    public class ApiManager : IApiManager
    {
        private readonly IDbContext<Application> _context;
        private readonly IDbContext<ApiDescriptor> _apis;
        private readonly IMemoryCache _cache;
        private readonly Type _cacheKey = typeof(Application);
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// 初始化类<see cref="ApiManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="apis">API描述数据库操作接口。</param>
        /// <param name="services">API服务接口列表。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="settingsManager">配置管理接口。</param>
        public ApiManager(IDbContext<Application> context, IDbContext<ApiDescriptor> apis, IEnumerable<IApiService> services, IMemoryCache cache, ISettingsManager settingsManager)
        {
            _context = context;
            _apis = apis;
            _cache = cache;
            _settingsManager = settingsManager;
            EnsureApiServices(services);
        }

        private void EnsureApiServices(IEnumerable<IApiService> services)
        {
            _apis.BeginTransaction(db =>
            {
                db.Update(new { Disabled = true });
                foreach (var service in services)
                {
                    if (!db.Any(x => x.Name == service.ApiName))
                        db.Create(new ApiDescriptor { Name = service.ApiName });
                }

                return true;
            });
        }

        /// <summary>
        /// 返回数据库操作结果，如果成功刷新缓存。
        /// </summary>
        /// <param name="result">操作结果。</param>
        /// <returns>返回数据操作结果。</returns>
        protected bool Result(bool result)
        {
            if (result)
                _cache.Remove(_cacheKey);
            return result;
        }

        /// <summary>
        /// 返回数据库操作结果，如果成功刷新缓存。
        /// </summary>
        /// <param name="result">操作结果。</param>
        /// <param name="success">成功枚举。</param>
        /// <returns>返回数据操作结果。</returns>
        protected DataResult Result(bool result, DataAction success)
        {
            if (result)
                _cache.Remove(_cacheKey);
            return DataResult.FromResult(result, success);
        }

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual DataResult GenerateToken(Application application)
        {
            application.Token = Cores.GeneralKey(128);
            application.ExpiredDate = DateTimeOffset.Now.AddDays(_settingsManager.GetSettings<ApiSettings>().TokenExpired);
            return Result(_context.Update(application.Id, new { application.Token, application.ExpiredDate }), DataAction.Updated);
        }

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual async Task<DataResult> GenerateTokenAsync(Application application)
        {
            application.Token = Cores.GeneralKey(128);
            application.ExpiredDate = DateTimeOffset.Now.AddDays(_settingsManager.GetSettings<ApiSettings>().TokenExpired);
            return Result(await _context.UpdateAsync(application.Id, new { application.Token, application.ExpiredDate }), DataAction.Updated);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual Application Find(int id)
        {
            var applications = LoadCache();
            return applications.Values.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual async Task<Application> FindAsync(int id)
        {
            var applications = await LoadCacheAsync();
            return applications.Values.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual Application Find(Guid appId)
        {
            var applications = LoadCache();
            applications.TryGetValue(appId, out var application);
            return application;
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual async Task<Application> FindAsync(Guid appId)
        {
            var applications = await LoadCacheAsync();
            applications.TryGetValue(appId, out var application);
            return application;
        }

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        public virtual IEnumerable<Application> Load(Predicate<Application> expression = null)
        {
            var applications = LoadCache().Values;
            if (expression == null)
                return applications;
            return applications.Where(x => expression(x)).ToList();
        }

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        public virtual async Task<IEnumerable<Application>> LoadAsync(Predicate<Application> expression = null)
        {
            var applications = await LoadCacheAsync();
            if (expression == null)
                return applications.Values;
            return applications.Values.Where(x => expression(x)).ToList();
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        public virtual IEnumerable<ApiDescriptor> LoadApis(int categoryId = 0)
        {
            var query = _apis.AsQueryable().Select(x => new { x.Id, x.CategoryId, x.Name, x.Disabled });
            if (categoryId > 0)
                query = query.Where(x => x.CategoryId == categoryId);
            return query.AsEnumerable();
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        public virtual Task<IEnumerable<ApiDescriptor>> LoadApisAsync(int categoryId = 0)
        {
            var query = _apis.AsQueryable().Select(x => new { x.Id, x.CategoryId, x.Name, x.Disabled });
            if (categoryId > 0)
                query = query.Where(x => x.CategoryId == categoryId);
            return query.AsEnumerableAsync();
        }

        private IDictionary<Guid, Application> LoadCache() =>
            _cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var applications = _context.Fetch();
                return applications.ToDictionary(x => x.AppId);
            });

        private async Task<IDictionary<Guid, Application>> LoadCacheAsync() =>
            await _cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var applications = await _context.FetchAsync();
                return applications.ToDictionary(x => x.AppId);
            });
    }
}