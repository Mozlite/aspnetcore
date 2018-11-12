using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 缓存对象管理基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">模型主键类型。</typeparam>
    public abstract class CachableObjectManager<TModel, TKey> : Mozlite.Extensions.CachableObjectManager<TModel, TKey>, ICachableObjectManager<TModel, TKey>
        where TModel : ISitableObject<TKey>
    {
        private readonly IMemoryCache _cache;
        private readonly ISiteContextAccessorBase _siteContextAccessor;
        /// <summary>
        /// 当前网站上下文接口。
        /// </summary>
        protected SiteContextBase Site => _siteContextAccessor.SiteContext;

        /// <summary>
        /// 缓存键。
        /// </summary>
        protected override object CacheKey => new Tuple<object, int>(base.CacheKey, Site.SiteId);

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        /// <param name="cancellationToken">取消标识。</param>
        public override Task<DataResult> SaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            if (model.SiteId == 0)
                model.SiteId = Site.SiteId;
            return base.SaveAsync(model, cancellationToken);
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public override DataResult Save(TModel model)
        {
            if (model.SiteId == 0)
                model.SiteId = Site.SiteId;
            return base.Save(model);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        public override IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            var models = _cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.Fetch(x => x.SiteId == Site.SiteId);
            });
            return models.Filter(expression);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回模型实例列表。</returns>
        public override async Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var models = await _cache.GetOrCreateAsync(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.FetchAsync(x => x.SiteId == Site.SiteId, cancellationToken);
            });
            return models.Filter(expression);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery ICachableObjectManager<TModel, TKey>.Load<TQuery>(TQuery query)
        {
            return Context.Load(query);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> ICachableObjectManager<TModel, TKey>.LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken)
        {
            return Context.LoadAsync(query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery Mozlite.Extensions.IObjectManager<TModel, TKey>.Load<TQuery>(TQuery query)
        {
            return Context.Load(query);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> Mozlite.Extensions.IObjectManager<TModel, TKey>.LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken)
        {
            return Context.LoadAsync(query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 初始化类<see cref="CachableObjectManager{TModel,TKey}"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        protected CachableObjectManager(IDbContext<TModel> db, IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor)
            : base(db, cache)
        {
            _cache = cache;
            _siteContextAccessor = siteContextAccessor;
        }
    }

    /// <summary>
    /// 缓存对象管理基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class CachableObjectManager<TModel> : CachableObjectManager<TModel, int>, ICachableObjectManager<TModel>
        where TModel : ISitableObject
    {
        /// <summary>
        /// 初始化类<see cref="CachableObjectManager{TModel}"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        protected CachableObjectManager(IDbContext<TModel> db, IMemoryCache cache, ISiteContextAccessorBase siteContextAccessor)
            : base(db, cache, siteContextAccessor)
        {
        }
    }
}