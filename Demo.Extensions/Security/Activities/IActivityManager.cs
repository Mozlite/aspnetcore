using System.Threading.Tasks;
using Mozlite.Extensions.Security.Activities;

namespace Demo.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理接口。
    /// </summary>
    public interface IActivityManager : IActivityManager<UserActivity>
    {
        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        UserActivityQuery Load(UserActivityQuery query);

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        Task<UserActivityQuery> LoadAsync(UserActivityQuery query);
    }
}