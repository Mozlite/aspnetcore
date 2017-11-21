using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 初始化类<see cref="GroupManager{TGroup}"/>。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupManager<TGroup> : CachableCategoryManager<TGroup>, IGroupManager<TGroup> where TGroup : GroupBase<TGroup>
    {
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 初始化类<see cref="GroupManager{TCategory}"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected GroupManager(IRepository<TGroup> repository, IMemoryCache cache)
            : base(repository, cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(TGroup category)
        {
            return Fetch().Any(x => x.ParentId == category.ParentId && x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 判断是否已经存在。
        /// </summary>
        /// <param name="category">分类实例。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<bool> IsDuplicatedAsync(TGroup category)
        {
            var groups = await FetchAsync();
            return groups.Any(x => x.ParentId == category.ParentId && x.Id != category.Id && x.Name == category.Name);
        }

        /// <summary>
        /// 加载所有的分类。
        /// </summary>
        /// <returns>返回分类列表。</returns>
        public override IEnumerable<TGroup> Fetch()
        {
            return _cache.GetOrCreate(typeof(TGroup), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var categories = Fetch();
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
        /// <returns>返回分类列表。</returns>
        public override async Task<IEnumerable<TGroup>> FetchAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(TGroup), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var categories = await FetchAsync();
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
    }
}