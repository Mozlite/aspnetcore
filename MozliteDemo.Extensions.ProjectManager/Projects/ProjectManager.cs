using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 项目管理实现类。
    /// </summary>
    public class ProjectManager : CachableObjectManager<Project>, IProjectManager
    {
        /// <summary>
        /// 初始化类<see cref="ProjectManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public ProjectManager(IDbContext<Project> context, IMemoryCache cache) : base(context, cache)
        {
        }
    }
}