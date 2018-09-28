using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 激活状态管理接口。
    /// </summary>
    /// <typeparam name="TActivity">当前活动状态类型。</typeparam>
    public interface IActivityManager<TActivity> : IActivityManagerBase
        where TActivity : UserActivity, new()
    {
        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(TActivity activity);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(TActivity activity);

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        TQuery Load<TQuery>(TQuery query)
            where TQuery : QueryBase<TActivity>;

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query)
            where TQuery : QueryBase<TActivity>;
    }
}