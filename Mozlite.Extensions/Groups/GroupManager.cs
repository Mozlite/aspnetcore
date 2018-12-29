using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Groups
{
    /// <summary>
    /// 初始化类<see cref="GroupManager{TGroup}"/>。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupManager<TGroup> : CachableObjectManager<TGroup>, IGroupManager<TGroup> where TGroup : GroupBase<TGroup>
    {
        /// <summary>
        /// 初始化类<see cref="GroupManager{TCategory}"/>。
        /// </summary>
        /// <param name="context">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected GroupManager(IDbContext<TGroup> context, IMemoryCache cache)
            : base(context, cache)
        {
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TGroup category)
        {
            var groups = Fetch(x => x.ParentId == category.ParentId && x.Id != category.Id && x.Name == category.Name);
            return groups.Any();
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TGroup category, CancellationToken cancellationToken = default)
        {
            var groups = await FetchAsync(x => x.ParentId == category.ParentId && x.Id != category.Id && x.Name == category.Name, cancellationToken);
            return groups.Any();
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回分类列表。</returns>
        public override IEnumerable<TGroup> Fetch(Expression<Predicate<TGroup>> expression = null)
        {
            var models = Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var categories = Context.Fetch();
                var dic = categories.ToDictionary(c => c.Id);
                dic[0] = Activator.CreateInstance<TGroup>();
                foreach (var category in categories)
                {
                    if (dic.TryGetValue(category.ParentId, out var temp))
                        temp.Add(category);
                }
                return dic.Values;
            });
            return models.Filter(expression);
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分类列表。</returns>
        public override async Task<IEnumerable<TGroup>> FetchAsync(Expression<Predicate<TGroup>> expression = null, CancellationToken cancellationToken = default)
        {
            var models = await Cache.GetOrCreateAsync(CacheKey, async ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                var categories = await Context.FetchAsync(cancellationToken: cancellationToken);
                var dic = categories.ToDictionary(c => c.Id);
                dic[0] = Activator.CreateInstance<TGroup>();
                foreach (var category in categories)
                {
                    if (dic.TryGetValue(category.ParentId, out var temp))
                        temp.Add(category);
                }
                return dic.Values;
            });
            return models.Filter(expression);
        }
    }
}