using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Data;
using Mozlite.Extensions;
using Mozlite.Extensions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="serviceProvider">服务提供者。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="settingsManager">配置管理接口。</param>
        public ApiManager(IServiceProvider serviceProvider, IMemoryCache cache, ISettingsManager settingsManager)
        {
            _context = serviceProvider.GetRequiredService<IDbContext<Application>>();
            _cacheContext = serviceProvider.GetRequiredService<IDbContext<CacheApplication>>();
            _apis = serviceProvider.GetRequiredService<IDbContext<ApiDescriptor>>();
            _cache = cache;
            _settingsManager = settingsManager;
            EnsureApiServices(serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>());
        }

        private void EnsureApiServices(IApiDescriptionGroupCollectionProvider provider)
        {
            var services = provider.ApiDescriptionGroups.Items
                .SelectMany(x => x.Items)
                .ToList();
            _apis.BeginTransaction(db =>
            {
                db.Update(new { Disabled = true });
                foreach (var service in services)
                {
                    var name = service.RelativePath.ToLower();
                    var description = service.GetDescription();
                    var descriptor = db.Find(x => x.Name == name);
                    if (descriptor == null)
                    {
                        descriptor = new ApiDescriptor
                        {
                            Name = name,
                            Description = description
                        };
                        db.Create(descriptor);
                    }
                    else
                    {
                        descriptor.Disabled = false;
                        if (description != null)
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

        /// <summary>
        /// 删除应用程序。
        /// </summary>
        /// <param name="ids">应用程序Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult DeleteApplications(int[] ids)
        {
            return Result(_context.Delete(x => x.Id.Included(ids)), DataAction.Deleted);
        }

        /// <summary>
        /// 删除应用程序。
        /// </summary>
        /// <param name="ids">应用程序Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteApplicationsAsync(int[] ids)
        {
            return Result(await _context.DeleteAsync(x => x.Id.Included(ids)), DataAction.Deleted);
        }

        /// <summary>
        /// 将API设置到应用中。
        /// </summary>
        /// <param name="appid">应用程序Id。</param>
        /// <param name="apis">API的Id列表。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult AddApis(int appid, int[] apis)
        {
            return Result(_context.BeginTransaction(db =>
            {
                var asdb = db.As<ApplicationService>();
                asdb.Delete(x => x.AppicationId == appid);
                if (apis?.Length > 0)
                {
                    foreach (var api in apis)
                    {
                        asdb.Create(new ApplicationService { AppicationId = appid, ServiceId = api });
                    }
                }

                return true;
            }), DataAction.Updated);
        }

        /// <summary>
        /// 将API设置到应用中。
        /// </summary>
        /// <param name="appid">应用程序Id。</param>
        /// <param name="apis">API的Id列表。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<DataResult> AddApisAsync(int appid, int[] apis)
        {
            return Result(await _context.BeginTransactionAsync(async db =>
            {
                var asdb = db.As<ApplicationService>();
                await asdb.DeleteAsync(x => x.AppicationId == appid);
                if (apis?.Length > 0)
                {
                    foreach (var api in apis)
                    {
                        await asdb.CreateAsync(new ApplicationService { AppicationId = appid, ServiceId = api });
                    }
                }

                return true;
            }), DataAction.Updated);
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