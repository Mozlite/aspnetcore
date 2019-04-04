using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Categories;

namespace MozliteDemo.Extensions.ProjectManager.Milestones
{
    /// <summary>
    /// 里程碑管理实现类。
    /// </summary>
    public class MilestoneManager : CachableCategoryManager<Milestone>, IMilestoneManager
    {
        /// <summary>
        /// 初始化类<see cref="MilestoneManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public MilestoneManager(IDbContext<Milestone> context, IMemoryCache cache)
            : base(context, cache)
        {
        }
    }
}