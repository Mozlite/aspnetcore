using Mozlite;
using Mozlite.Extensions.Categories;

namespace MozliteDemo.Extensions.ProjectManager.Milestones
{
    /// <summary>
    /// 里程碑管理接口。
    /// </summary>
    public interface IMilestoneManager : ICachableCategoryManager<Milestone>, ISingletonService
    {

    }
}