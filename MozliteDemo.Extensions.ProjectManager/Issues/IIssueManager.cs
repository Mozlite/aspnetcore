using Mozlite;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 任务管理接口。
    /// </summary>
    public interface IIssueManager : IObjectManager<Issue>, ISingletonService
    {

    }
}