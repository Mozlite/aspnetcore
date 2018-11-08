using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions;
using Mozlite.Extensions.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序管理。
    /// </summary>
    public class ApiManager : IApiManager
    {
        private readonly IDbContext<Application> _context;
        private readonly IDbContext<CacheApplication> _cacheContext;
        private readonly IDbContext<ApiDescriptor> _apis;
        private readonly IMemoryCache _cache;
        private readonly Type _cacheKey = typeof(CacheApplication);
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// 初始化类<see cref="ApiManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cacheContext">缓存上下文。</param>
        /// <param name="apis">API描述数据库操作接口。</param>
        /// <param name="services">API服务接口列表。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="settingsManager">配置管理接口。</param>
        public ApiManager(IDbContext<Application> context, IDbContext<CacheApplication> cacheContext, IDbContext<ApiDescriptor> apis, IEnumerable<IApiService> services, IMemoryCache cache, ISettingsManager settingsManager)
        {
            _context = context;
            _cacheContext = cacheContext;
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
                    var description = service.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description;
                    var descriptor = db.Find(x => x.Name == service.ApiName);
                    if (descriptor == null)
                    {
                        descriptor = new ApiDescriptor
                        {
                            Name = service.ApiName,
                            Description = description
                        };
                        db.Create(descriptor);
                    }
                    else
                    {
                        descriptor.Disabled = false;
                        descriptor.Description = description;
                        db.Update(descriptor);
                    }
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
        public virtual DataResult GenerateToken(CacheApplication application)
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
        public virtual async Task<DataResult> GenerateTokenAsync(CacheApplication application)
        {
            application.Token = Cores.GeneralKey(128);
            application.ExpiredDate = DateTimeOffset.Now.AddDays(_settingsManager.GetSettings<ApiSettings>().TokenExpired);
            return Result(await _context.UpdateAsync(application.Id, new { application.Token, application.ExpiredDate }), DataAction.Updated);
        }

        /// <summary>
        /// 保存应用程序。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual DataResult Save(Application application)
        {
            if (_context.Any(
                x => x.Id != application.Id && x.Name == application.Name && x.UserId == application.UserId))
                return DataAction.Duplicate;
            if (application.Id > 0)
                return DataResult.FromResult(_context.Update(application), DataAction.Updated);
            return DataResult.FromResult(_context.Create(application), DataAction.Created);
        }

        /// <summary>
        /// 保存应用程序。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual async Task<DataResult> SaveAsync(Application application)
        {
            if (await _context.AnyAsync(
                x => x.Id != application.Id && x.Name == application.Name && x.UserId == application.UserId))
                return DataAction.Duplicate;
            if (application.Id > 0)
                return DataResult.FromResult(await _context.UpdateAsync(application), DataAction.Updated);
            return DataResult.FromResult(await _context.CreateAsync(application), DataAction.Created);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual Application Find(int id)
        {
            return _context.Find(id);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual Task<Application> FindAsync(int id)
        {
            return _context.FindAsync(id);
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual CacheApplication Find(Guid appId)
        {
            var applications = LoadCache();
            if (applications.TryGetValue(appId, out var application))
                return application;
            return null;
        }

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        public virtual async Task<CacheApplication> FindAsync(Guid appId)
        {
            var applications = await LoadCacheAsync();
            if (applications.TryGetValue(appId, out var application))
                return application;
            return null;
        }

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        public virtual IEnumerable<Application> Load(Expression<Predicate<Application>> expression = null)
        {
            return _context.Fetch(expression);
        }

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        public virtual Task<IEnumerable<Application>> LoadAsync(Expression<Predicate<Application>> expression = null)
        {
            return _context.FetchAsync(expression);
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        public virtual IEnumerable<ApiDescriptor> LoadApis(int categoryId = 0)
        {
            if (categoryId > 0)
                return _apis.Fetch(x => x.CategoryId == categoryId);
            return _apis.Fetch();
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        public virtual Task<IEnumerable<ApiDescriptor>> LoadApisAsync(int categoryId = 0)
        {
            if (categoryId > 0)
                return _apis.FetchAsync(x => x.CategoryId == categoryId);
            return _apis.FetchAsync();
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="applicationId">应用程序Id。</param>
        /// <returns>返回当前应用程序的API列表。</returns>
        public virtual IEnumerable<ApiDescriptor> LoadApplicationApis(int applicationId)
        {
            return _apis.AsQueryable().InnerJoin<ApplicationService>((a, s) => a.Id == s.ServiceId)
                .Where<ApplicationService>(x => x.AppicationId == applicationId)
                .AsEnumerable();
        }

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="applicationId">应用程序Id。</param>
        /// <returns>返回当前应用程序的API列表。</returns>
        public virtual Task<IEnumerable<ApiDescriptor>> LoadApplicationApisAsync(int applicationId)
        {
            return _apis.AsQueryable().InnerJoin<ApplicationService>((a, s) => a.Id == s.ServiceId)
                .Where<ApplicationService>(x => x.AppicationId == applicationId)
                .AsEnumerableAsync();
        }

        private IDictionary<Guid, CacheApplication> LoadCache() =>
            _cache.GetOrCreate(_cacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var applications = _cacheContext.Fetch();
                return applications.ToDictionary(x => x.AppId);
            });

        private async Task<IDictionary<Guid, CacheApplication>> LoadCacheAsync() =>
            await _cache.GetOrCreateAsync(_cacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var applications = await _cacheContext.FetchAsync();
                return applications.ToDictionary(x => x.AppId);
            });
    }
}