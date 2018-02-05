using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Groups
{
    /// <summary>
    /// 初始化类<see cref="GroupExManager{TGroup}"/>。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupExManager<TGroup> : CachableCategoryExManager<TGroup>, IGroupExManager<TGroup> where TGroup : GroupExBase<TGroup>
    {
        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TGroup category)
        {
            return Categories.Any(x => x.ParentId == category.ParentId && x.SiteId == Site.SiteId && x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TGroup category, CancellationToken cancellationToken = default)
        {
            var groups = await FetchAsync(cancellationToken: cancellationToken);
            return groups.Any(x => x.ParentId == category.ParentId && x.SiteId == Site.SiteId && x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回分类列表。</returns>
        public override IEnumerable<TGroup> Fetch(Expression<Predicate<TGroup>> expression = null)
        {
            return Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var categories = Context.Fetch(x => x.SiteId == Site.SiteId);
                var dic = categories.ToDictionary(c => c.Id);
                dic[0] = Activator.CreateInstance<TGroup>();
                foreach (var category in categories)
                {
                    if (dic.TryGetValue(category.ParentId, out var temp))
                        temp.Add(category);
                }
                return dic.Values;
            });
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分类列表。</returns>
        public override async Task<IEnumerable<TGroup>> FetchAsync(Expression<Predicate<TGroup>> expression = null, CancellationToken cancellationToken = default)
        {
            return await Cache.GetOrCreateAsync(CacheKey, async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var categories = await Context.FetchAsync(x => x.SiteId == Site.SiteId, cancellationToken);
                var dic = categories.ToDictionary(c => c.Id);
                dic[0] = Activator.CreateInstance<TGroup>();
                foreach (var category in categories)
                {
                    if (dic.TryGetValue(category.ParentId, out var temp))
                        temp.Add(category);
                }
                return dic.Values;
            });
        }

        /// <summary>
        /// 初始化类<see cref="GroupExManager{TGroup}"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        /// <param name="cache">缓存接口。</param>
        protected GroupExManager(IDbContext<TGroup> db, ISiteContextAccessorBase siteContextAccessor, IMemoryCache cache)
            : base(db, siteContextAccessor, cache)
        {
        }
    }
}