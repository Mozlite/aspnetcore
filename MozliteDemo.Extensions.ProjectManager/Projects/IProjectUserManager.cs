using Mozlite;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 项目管理接口。
    /// </summary>
    public interface IProjectUserManager : ICachableObjectManager<ProjectUser>, ISingletonService
    {
        /// <summary>
        /// 保存用户.
        /// </summary>
        /// <param name="ids">用户Id列表.</param>
        /// <returns>返回保存结果.</returns>
        bool SaveUsers(int[] ids);
    }
}
