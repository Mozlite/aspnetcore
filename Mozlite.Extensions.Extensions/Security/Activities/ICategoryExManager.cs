using Mozlite.Data;
using Mozlite.Extensions.Categories;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类管理接口。
    /// </summary>
    public interface ICategoryExManager : ICategoryExManager<CategoryEx>, ISingletonService
    {

    }

    /// <summary>
    /// 分类实现类。
    /// </summary>
    [Suppress(typeof(CategoryManager))]
    public class CategoryExManager : CategoryExManager<CategoryEx>, ICategoryExManager
    {
        /// <summary>
        /// 初始化类<see cref="CategoryExManager"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        public CategoryExManager(IDbContext<CategoryEx> db, ISiteContextAccessorBase siteContextAccessor)
            : base(db, siteContextAccessor)
        {
        }
    }
}