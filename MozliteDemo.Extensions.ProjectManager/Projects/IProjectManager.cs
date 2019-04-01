using Mozlite;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 项目管理接口。
    /// </summary>
    public interface IProjectManager : ICachableObjectManager<Project>, ISingletonService
    {

    }
}