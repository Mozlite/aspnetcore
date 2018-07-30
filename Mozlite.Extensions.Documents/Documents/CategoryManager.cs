using System;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Groups;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 分类管理实现类。
    /// </summary>
    public class CategoryManager : GroupManager<Category>, ICategoryManager
    {
        /// <summary>
        /// 初始化类<see cref="CategoryManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口实例。</param>
        /// <param name="cache">缓存接口。</param>
        public CategoryManager(IDbContext<Category> context, IMemoryCache cache) 
            : base(context, cache)
        {
        }
    }
}
