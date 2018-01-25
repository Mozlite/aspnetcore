using System.Threading.Tasks;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态。
    /// </summary>
    public interface IActivityManager : ISingletonService
    {
        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(UserActivity activity);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(UserActivity activity);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(string activity, int userId);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(string activity, int userId);

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <typeparam name="TUser">用户类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        TQuery Load<TQuery, TUser>(TQuery query)
            where TQuery : UserActivityQuery<TUser>
            where TUser : UserBase;

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <typeparam name="TUser">用户类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        Task<TQuery> LoadAsync<TQuery, TUser>(TQuery query)
            where TQuery : UserActivityQuery<TUser>
            where TUser : UserBase;
    }
}