using Mozlite.Data;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 任务管理实现类。
    /// </summary>
    public class IssueManager : ObjectManager<Issue>, IIssueManager
    {
        /// <summary>
        /// 初始化类<see cref="IssueManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        public IssueManager(IDbContext<Issue> context) : base(context)
        {
        }
    }
}