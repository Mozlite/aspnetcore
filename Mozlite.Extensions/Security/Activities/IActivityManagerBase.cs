using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态。
    /// </summary>
    public interface IActivityManagerBase
    {
        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        Task<bool> CreateAsync(int categoryId, string activity, int userId = 0);

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        bool Create(int categoryId, string activity, int userId = 0);
    }
}