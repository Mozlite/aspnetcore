using System.Collections.Generic;
using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类管理类实现基类。
    /// </summary>
    /// <typeparam name="TCategory">分类类型。</typeparam>
    public abstract class CategoryExManager<TCategory> : ObjectExManager<TCategory>, ICategoryExManager<TCategory>
        where TCategory : CategoryExBase
    {
        /// <summary>
        /// 初始化类<see cref="CategoryExManager{TCategory}"/>。
        /// </summary>
        /// <param name="db">数据库操作实例。</param>
        /// <param name="siteContextAccessor">当前网站访问接口。</param>
        protected CategoryExManager(IDbContext<TCategory> db, ISiteContextAccessorBase siteContextAccessor) : base(db, siteContextAccessor)
        {
        }

        /// <summary>
        /// 当前分类实例。
        /// </summary>
        public virtual IEnumerable<TCategory> Categories => Context.Fetch();
    }
}