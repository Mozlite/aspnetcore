using System.Collections.Generic;
using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
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
        /// <param name="context">数据库操作实例。</param>
        protected CategoryManager(IDbContext<TCategory> context) : base(context)
        {
        }

        /// <summary>
        /// 当前分类实例。
        /// </summary>
        public virtual IEnumerable<TCategory> Categories => Context.Fetch();
    }
}