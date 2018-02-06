using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类管理实现类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryManager<TCategory> : CachableCategoryManager<TCategory>, ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 初始化类<see cref="CategoryManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CategoryManager(IDbContext<TCategory> db, IMemoryCache cache) : base(db, cache)
        {
        }
    }
}