using Mozlite.Data;
using System.Collections.Generic;

namespace Mozlite.Extensions.Extensions.Categories
{
    /// <summary>
    /// 分类管理类实现基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryManager<TCategory> : ObjectManager<TCategory>, ICategoryManager<TCategory>
        where TCategory : CategoryBase
    {
        /// <summary>
        /// 初始化类<see cref="CategoryManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        protected CategoryManager(IDbContext<TCategory> db, ISiteContextAccessorBase siteContextAccessor) 
            : base(db, siteContextAccessor)
        {
        }

        /// <summary>
        /// 当前分类实例。
        /// </summary>
        public virtual IEnumerable<TCategory> Categories => Fetch();
    }
}