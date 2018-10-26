using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 分类接口。
    /// </summary>
    public interface ICategoryManager : ICachableCategoryManager<Category>, ISingletonService
    {

    }

    /// <summary>
    /// 分类实现类。
    /// </summary>
    public class CategoryManager : CachableCategoryManager<Category>, ICategoryManager
    {
        /// <summary>
        /// 初始化类<see cref="CategoryManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public CategoryManager(IDbContext<Category> context, IMemoryCache cache) : base(context, cache)
        {
        }
    }
}