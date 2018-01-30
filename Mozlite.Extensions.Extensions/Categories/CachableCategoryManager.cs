using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 缓存分类管理实现类基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CachableCategoryManager<TCategory> : CategoryExManager<TCategory>,
        ICachableCategoryManager<TCategory>
        where TCategory : CategoryExBase
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化类<see cref="CachableCategoryManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CachableCategoryManager(IDbContext<TCategory> db, IMemoryCache cache)
            : base(db)
        {
            _cache = cache;
        }

        /// <summary>
        /// 根据结果刷新缓存。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        /// <returns>返回当前结果实例。</returns>
        protected DataResult RefreshCache(DataResult result)
        {
            if (result.Succeed())
                _cache.Remove(typeof(TCategory));
            return result;
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        public override DataResult Save(TCategory category)
        {
            return RefreshCache(base.Save(category));
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回保存结果。</returns>
        public override async Task<DataResult> SaveAsync(TCategory category)
        {
            return RefreshCache(await base.SaveAsync(category));
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TCategory category)
        {
            return Fetch().Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TCategory category)
        {
            var categories = await FetchAsync();
            return categories.Any(x => x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(int id)
        {
            return RefreshCache(base.Delete(id));
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(int id)
        {
            return RefreshCache(await base.DeleteAsync(id));
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(string ids)
        {
            return RefreshCache(await base.DeleteAsync(ids));
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        /// <param name="ids">分类Id集合，以“,”分隔。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(string ids)
        {
            return RefreshCache(base.Delete(ids));
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        public override IEnumerable<TCategory> Fetch()
        {
            return _cache.GetOrCreate(typeof(TCategory), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return base.Fetch();
            });
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        public override Task<IEnumerable<TCategory>> FetchAsync()
        {
            return _cache.GetOrCreateAsync(typeof(TCategory), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return await base.FetchAsync();
            });
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public override TCategory Get(int id)
        {
            return Fetch().FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public override async Task<TCategory> GetAsync(int id)
        {
            var categories = await FetchAsync();
            return categories.FirstOrDefault(x => x.Id == id);
        }
    }
}