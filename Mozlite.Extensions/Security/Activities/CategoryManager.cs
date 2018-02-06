using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类管理实现类。
    /// </summary>
    public class CategoryManager : CachableCategoryManager<Category>, ICategoryManager
    {
        /// <summary>
        /// 初始化类<see cref="CategoryManager"/>。
        /// </summary>
        /// <param name="db">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        public CategoryManager(IDbContext<Category> db, IMemoryCache cache) : base(db, cache)
        {
        }
    }
}