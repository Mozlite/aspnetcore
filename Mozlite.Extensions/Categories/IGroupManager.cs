using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分组管理接口。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public interface IGroupManager<TGroup> : ICachableCategoryManager<TGroup>
        where TGroup : GroupBase<TGroup>
    {

    }

    /// <summary>
    /// 初始化类<see cref="GroupManager{TGroup}"/>。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupManager<TGroup> : CachableCategoryManager<TGroup>, IGroupManager<TGroup> where TGroup : GroupBase<TGroup>
    {
        /// <summary>
        /// 初始化类<see cref="GroupManager{TCategory}"/>。
        /// </summary>
        /// <param name="repository">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected GroupManager(IRepository<TGroup> repository, IMemoryCache cache)
            : base(repository, cache)
        {
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
    }
}